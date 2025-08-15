using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerStat playerStat;

    public string attackState;
    public Animator animator;
    public List<string> attackAnimations; // List of animation names to play

    [Header("VFX")]
    public ParticleSystem auraTakeFruitVFX;
    public ParticleSystem ultiVFX;
    public ParticleSystem ultiVFX2;//Use for vfx dot ulti
    public ParticleSystem hitImpact;
    public ParticleSystem bloodSplash;
    public Transform displayDealtDamTransform; // Transform to display dealt damage text
    //public ParticleSystem[] beingHitText;
    //public ParticleSystem damageDisplay;
    //private CFXR_ParticleText particleDamagePrefab;

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

        //particleDamagePrefab = damageDisplay.GetComponent<CFXR_ParticleText>();
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
            yield return new WaitForSeconds(0.75f); // Thời gian giữa các đòn tấn công
        }

        if (isHavingSpecialAttack)
        {
            yield return new WaitForSeconds(0.3f);
            attackState = "DoneAttack";
            animator.SetTrigger(specialAttackHash);
            yield return new WaitForSeconds(1f); // Thời gian cho đòn tấn công đặc biệt
        }

        if (PlayerUltimate.instance.isUltimateValid)
        {
            PlayerUltimate.instance.totalRound--;
        }

        // Gọi hàm DoneAttack sau khi kết thúc chuỗi tấn công
        StartCoroutine(DoneAttack(GameManager.instance.CheckIfCurrentEnemyDead(), GameManager.instance.CheckIfAllEnemiesDead()));
    }

    public IEnumerator DoneAttack(bool isEnemyDie, bool isAllEnemiesDie)
    {
        StopTakeFruitVFX(); // Dừng hiệu ứng khi kết thúc tấn công
        yield return new WaitForSeconds(0.5f); // Thời gian nghỉ sau khi ra hết đòn
        animator.SetTrigger(doneAttackHash); // Kết thúc chuỗi tấn công
        yield return new WaitForSeconds(0.5f); // Thời gian chờ anim chạy về done attack hoặc đợi cam thực hiện xong anim, tối thiểu đợi 0.5f

        //Check if all enemies die
        if (isAllEnemiesDie)
        {
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.currentTurn = "Win";
            GameManager.instance.currentGameState = GameState.GameOver;
            Victory();
        }
        //Check if current enemy die
        else if (isEnemyDie)
        {
            yield return new WaitForSeconds(0.5f);
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
        StartCoroutine(CameraManager.instance.SetVerticalFOV(30f, 0.5f));
        StartCoroutine(CameraManager.instance.SetFollowOffset(0.5f, 'X', 0f));

        UIManager.instance.ShowGameOverPanel(true);
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

    public void ResetAnimState()
    { 
        animator.SetBool(isDeadHash, false); // Reset dead animation state
        animator.SetBool(isVictoryHash, false); // Reset victory animation state
       // animator.SetTrigger(doneAttackHash); // Reset done attack state
    }

    public void PlayTakeFruitVFX()
    {
        if (auraTakeFruitVFX != null)
        {
            auraTakeFruitVFX.Play(); // Play the aura effect when taking fruit
        }
    }

    public void StopTakeFruitVFX()
    {
        if (auraTakeFruitVFX != null)
        {
            auraTakeFruitVFX.Stop(); // Stop the aura effect
        }
    }

    public void PlayUltiVFX()
    {
        if (ultiVFX != null)
        {
            ultiVFX.Play(); // Play the ultimate visual effect
        }
    }

    public void StopUltiVFX()
    {
        if (ultiVFX != null)
        {
            ultiVFX.Stop(); // Stop the ultimate visual effect
        }
    }

    public void PlayUltiVFX2()
    {
        if (ultiVFX2 != null)
        {
            ultiVFX2.Play(); // Play the second ultimate visual effect
        }
    }

    public void StopUltiVFX2()
    {
        if (ultiVFX2 != null)
        {
            ultiVFX2.Stop(); // Stop the second ultimate visual effect
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyHit"))
        {
            EnemyStat enemyStat = other.GetComponentInParent<EnemyStat>();
            if (enemyStat != null)
            {
                PlayerUltimate.instance.AddMana(5);

                float dam = NumberFomatter.RoundFloatToTwoDecimalPlaces(enemyStat.damage);

                playerStat.TakeDamage(dam);
                CameraManager.instance.StartCoroutine(CameraManager.instance.ShakeCamera(3f, 3f, 0.5f));
                UIManager.instance.DisplayDamageText(bloodSplash.transform, displayDealtDamTransform, dam); // Display damage text
                //DisplayDamageText(dam); // Display damage text
                BeingAttactk(); 

                if (playerStat.CheckIfObjectDead())
                { 
                    var enemyAttack = other.GetComponentInParent<EnemyAttack>();
                    if (enemyAttack != null)
                    {
                        if (enemyAttack.attackState == "DoneAttack")
                        {
                            Debug.Log("Player is dead");
                            animator.SetBool(isDeadHash, true); // Trigger the dead animation
                            CameraManager.instance.StartCoroutine(CameraManager.instance.SetCamWhenTargetDie(true, 3, -8f));
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

    public void BeingAttactk()
    {
        hitImpact.Play();
        bloodSplash.Play();
    }

    //public void DisplayDamageText(float damage)
    //{
    //    string damageText = "-" + NumberFomatter.FormatFloatToString(damage, 2);
    //    //particleDamagePrefab.UpdateText(damageText); // Update the text in the prefab
    //    //damageDisplay.Play(); // Play the damage display effect
    //}
}
