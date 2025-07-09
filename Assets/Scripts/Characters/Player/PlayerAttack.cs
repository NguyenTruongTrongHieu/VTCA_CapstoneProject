using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerStat playerStat;

    public string attackState;
    public Animator animator;
    public List<string> attackAnimations; // List of animation names to play
    private List<int> attackHashes = new List<int>();//Trigger
    private int specialAttackHash;//Trigger
    private int doneAttackHash; // Trigger for done attack
    private int getHitHash; // Trigger for getting hit
    private int isDeadHash; // Trigger for dead animation
    private int isVictoryHash; // Trigger for victory animation

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        specialAttackHash = Animator.StringToHash("specialAttack");
        doneAttackHash = Animator.StringToHash("DoneAttack");
        getHitHash = Animator.StringToHash("GetHit");
        isDeadHash = Animator.StringToHash("isDead");
        isVictoryHash = Animator.StringToHash("isVictory");
        for (int i = 0; i < attackAnimations.Count; i++)
        {
            attackHashes.Add(Animator.StringToHash(attackAnimations[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentTurn == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(PlayAttackSequence(1, false));
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(PlayAttackSequence(4, true));
            }
        }
    }

    public IEnumerator PlayAttackSequence(int totalHits, bool isHavingSpecialAttack)
    {
        GameManager.instance.currentTurn = "None"; // Set turn to None while attacking
        attackState = "Attacking"; // Set attack state
        for (int i = 0; i < totalHits; i++)
        {
            int index = i % attackHashes.Count;
            int hash = attackHashes[index];

            animator.SetTrigger(hash);

            if (!isHavingSpecialAttack && i == totalHits - 1)
            { 
                attackState = "DoneAttack"; // Set attack state to DoneAttack if not having special attack
            }

            // Chờ đến khi animator qua state khác hoặc anim kết thúc
            //yield return new WaitUntil(() =>
            //{
            //    AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            //    return  info.normalizedTime >= 1f;
            //});
            yield return new WaitForSeconds(0.5f); // Thời gian giữa các đòn tấn công
        }

        if (isHavingSpecialAttack)
        {
            animator.SetTrigger(specialAttackHash);
            attackState = "DoneAttack";
            yield return new WaitForSeconds(1f); // Thời gian cho đòn tấn công đặc biệt
        }

        // Gọi hàm DoneAttack sau khi kết thúc chuỗi tấn công
        StartCoroutine(DoneAttack(GameManager.instance.CheckIfCurrentEnemyDead(), GameManager.instance.CheckIfAllEnemiesDead()));
    }

    public IEnumerator DoneAttack(bool isEnemyDie, bool isAllEnemiesDie)
    {
        yield return new WaitForSeconds(0.5f); // Thời gian nghỉ sau khi ra hết đòn
        animator.SetTrigger(doneAttackHash); // Kết thúc chuỗi tấn công

        //Check if all enemies die
        if (isAllEnemiesDie)
        {
            GameManager.instance.currentTurn = "Win";
            GameManager.instance.currentGameState = GameState.GameOver;
            Victory();
        }
        //Check if current enemy die
        else if (isEnemyDie)
        {
            GameManager.instance.currentTurn = "None";
            GameManager.instance.GoToNextEnemy(); // Go to next enemy
            GetComponent<Player>().SetUpBehaviourTree();
        }
        else
        {
            GameManager.instance.currentTurn = "Enemy";
        }
    }

    public void Victory()
    {
        animator.SetBool(isVictoryHash, true);
        StartCoroutine(RotateToTarget(GetComponent<Player>().startQuaternion));
        StartCoroutine(
        //CameraManager.instance.SetScreenPosComposition(1f, true, 0f));
        CameraManager.instance.SetHardLookAt(1f, 'Z', 0f));
    }

    public IEnumerator RotateToTarget(Vector3 targetRot)
    {
        float duration = 0.5f; // Duration of the rotation
        float elapsedTime = 0f;
        Quaternion targetRotation = Quaternion.Euler(targetRot);

        Quaternion initialRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation; // Ensure final rotation is set
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyHit"))
        {
            EnemyStat enemyStat = other.GetComponentInParent<EnemyStat>();
            if (enemyStat != null)
            {
                Debug.Log("Enemy hit");
                playerStat.TakeDamage(enemyStat.damage);
                if (playerStat.CheckIfObjectDead())
                { 
                    var enemyAttack = other.GetComponentInParent<EnemyAttack>();
                    if (enemyAttack != null)
                    {
                        if (enemyAttack.attackState == "DoneAttack")
                        {
                            Debug.Log("Player is dead");
                            animator.SetBool(isDeadHash, true); // Trigger the dead animation
                        }
                        else
                        {
                            Debug.Log("Player is hit but not dead");
                        }
                    }
                }

                if (!animator.GetBool(isDeadHash))
                {
                    animator.SetTrigger(getHitHash); // Trigger the get hit animation
                }
            }
        }
    }
}
