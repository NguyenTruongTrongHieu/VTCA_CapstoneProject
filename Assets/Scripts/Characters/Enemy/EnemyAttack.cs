using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public EnemyStat enemyStat;

    public string attackState;//"": start; "Attacking": attacking; "DoneAttack": done animation attack; "DoneCircleAttack": done all attack, done setup for player attack

    public Animator animator;
    public List<string> attackAnimations; // List of animation names to play
    private List<int> attackHashes = new List<int>();//Trigger
    private int doneAttackHash; // Trigger for done attack
    private int getHitHash; // Trigger for getting hit
    private int isDeadHash; // Trigger for dead animation
    private int isVictoryHash;

    private int specialAttackHash;//Trigger, maybe use in the future

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackState = "";
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
        if (GameManager.instance.currentGameState == GameState.Playing && GameManager.instance.currentTurn == "Enemy" &&( attackState != "Attacking" && attackState != "DoneAttack"))
        { 
            attackState = "Attacking";
            StartCoroutine(PlayAttackSequence(Random.Range(1, 4), false));
        }
    }

    public IEnumerator PlayAttackSequence(int totalHits, bool isHavingSpecialAttack)
    {
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
            attackState = "DoneAttack"; // Đặt lại trạng thái tấn công
            yield return new WaitForSeconds(1f); // Thời gian cho đòn tấn công đặc biệt
        }

        // Gọi hàm DoneAttack sau khi kết thúc chuỗi tấn công
        StartCoroutine(DoneAttack(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStat>().CheckIfObjectDead()));
    }

    public IEnumerator DoneAttack(bool isPlayerDie)
    {
        yield return new WaitForSeconds(0.5f); // Thời gian nghỉ sau khi ra hết đòn
        animator.SetTrigger(doneAttackHash); // Kết thúc chuỗi tấn công

        //Check if current enemy die
        if (isPlayerDie)
        {
            GameManager.instance.currentTurn = "Lose";
            Victory();
        }
        else
        {
            GameManager.instance.currentTurn = "Player";
        }

        attackState = "DoneCircleAttack"; // Đặt lại trạng thái tấn công
    }

    public void Victory()
    {
        animator.SetBool(isVictoryHash, true);
        StartCoroutine(RotateToTarget(GameManager.instance.enemiesEndRotation));
    }

    public IEnumerator RotateToTarget(Vector3 targetRot)
    {
        float duration = 0.5f; // Duration of the rotation
        float elapsedTime = 0f;
        Quaternion targetRotation = Quaternion.Euler(targetRot);
        Debug.Log("Rotate to target: " + targetRotation.eulerAngles);

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
        if (other.CompareTag("PlayerHit"))
        {
            PlayerStat playerStat = other.GetComponentInParent<PlayerStat>();
            if (playerStat != null)
            {
                Debug.Log("Player hit");
                enemyStat.TakeDamage(playerStat.damage);
                if (enemyStat.CheckIfObjectDead())
                {
                    var playerAttack = other.GetComponentInParent<PlayerAttack>();
                    if (playerAttack != null)
                    {
                        if (playerAttack.attackState == "DoneAttack")
                        {
                            Debug.Log("Player is dead");
                            animator.SetBool(isDeadHash, true); // Trigger dead animation
                            Destroy(gameObject, 0.8f);
                        }
                    }
                }
                if (!animator.GetBool(isDeadHash))
                {
                    animator.SetTrigger(getHitHash); // Trigger the get hit animation
                }
            }
            else
            { 
                Debug.LogWarning("PlayerStat component not found on the player object.");
            }
        }

        if (other.CompareTag("PlayerSpecialHit"))
        {
            PlayerStat playerStat = other.GetComponentInParent<PlayerStat>();
            if (playerStat != null)
            {
                Debug.Log("Player special hit");
                enemyStat.TakeDamage(playerStat.damage * 3);
                if (enemyStat.CheckIfObjectDead())
                {
                    var playerAttack = other.GetComponentInParent<PlayerAttack>();
                    if (playerAttack != null)
                    {
                        if (playerAttack.attackState == "DoneAttack")
                        {
                            Debug.Log("Enemy is dead");
                            animator.SetBool(isDeadHash, true); // Trigger dead animation
                            Destroy(gameObject, 0.8f);
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
