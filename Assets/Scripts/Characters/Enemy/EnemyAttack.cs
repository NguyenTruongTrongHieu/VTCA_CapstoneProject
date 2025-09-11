using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyAttack : MonoBehaviour
{
    public EnemyStat enemyStat;

    public string attackState;//"": start; "Attacking": attacking; "DoneAttack": done animation attack; "DoneCircleAttack": done all attack, done setup for player attack

    [Header("SFX")]
    public string[] bossVictoryVoice;
    public string[] enemyHitVoice;
    public string[] enemyDieVoice;

    [Header("VFX")]
    public ParticleSystem dropCoinNormalVFX;
    public ParticleSystem dropCoinSpecialVFX;
    public ParticleSystem dropCrystalVFX;
    public ParticleSystem dropStarVFX;
    public ParticleSystem debuffVFX;
    public ParticleSystem hitImpact;
    public ParticleSystem hitImpactSpecial;
    public ParticleSystem bloodSplash;
    public Transform displayDealtDamTransform; // Transform to display dealt damage text
    //public ParticleSystem[] beingHitText;
    //public ParticleSystem damageDisplay;
    //private CFXR_ParticleText particleDamagePrefab;

    [Header("Count turn")]
    public int enemyDebuffTurn;
    public int enemyBuffTurn;

    [Header("Animation")]
    public Animator animator;
    public List<string> attackAnimations; // List of animation names to play
    private List<int> attackHashes = new List<int>();//Trigger
    private int doneAttackHash; // Trigger for done attack
    private int getHitHash; // Trigger for getting hit
    private int isDeadHash; // Trigger for dead animation
    private int isVictoryHash;

    public float dieAnimDuration = 2f;

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

        //particleDamagePrefab = damageDisplay.GetComponent<CFXR_ParticleText>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState == GameState.Playing && GameManager.instance.currentTurn == "Enemy" &&( attackState != "Attacking" && attackState != "DoneAttack"))
        { 
            attackState = "Attacking";
            StartCoroutine(PlayAttackSequence(Random.Range(1, 4), false));//Random.Range(1, 4)
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
            yield return new WaitForSeconds(0.75f); // Thời gian giữa các đòn tấn công
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
        if (enemyDebuffTurn > 0)
        { 
            enemyDebuffTurn--;
            if (enemyDebuffTurn <= 0)
            {
                debuffVFX.Stop();

                enemyStat.SetUpDefense();
            }
        }

        yield return new WaitForSeconds(0.5f); // Thời gian nghỉ sau khi ra hết đòn
        animator.SetTrigger(doneAttackHash); // Kết thúc chuỗi tấn công

        //Check if current enemy die
        if (isPlayerDie)
        {
            yield return new WaitForSeconds(1.15f);
            Victory();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.currentTurn = "Player";
        }

        attackState = "DoneCircleAttack"; // Đặt lại trạng thái tấn công
    }

    public void Victory()
    {
        if (this.transform != LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform)
        {
            return;
        }

        GameManager.instance.currentTurn = "Lose";
        GameManager.instance.currentGameState = GameState.GameOver;

        animator.SetBool(isVictoryHash, true);
        StartCoroutine(RotateToTarget(GameManager.instance.enemiesEndRotation));

        CameraManager.instance.StartCoroutine(CameraManager.instance.SetTargetForCam(LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform, 0f));
        CameraManager.instance.StartCoroutine(CameraManager.instance.SetHardLookAt(1f, 'Z', 0f));
        CameraManager.instance.StartCoroutine(CameraManager.instance.SetVerticalFOV(30f, 0.5f));
        CameraManager.instance.StartCoroutine(CameraManager.instance.SetFollowOffset(0.5f, 'X', 0f));

        if (enemyStat.enemyType == EnemyType.boss)
        {
            int randomIndex = Random.Range(0, bossVictoryVoice.Length);
            AudioManager.instance.PlaySFX(bossVictoryVoice[randomIndex]);
        }

        UIManager.instance.HideAllHUD();
        UIManager.instance.ShowGameOverPanel(false);
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

    public void GetHitAnim()
    {
        animator.SetTrigger(getHitHash);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHit"))
        {
            PlayerStat playerStat = other.GetComponentInParent<PlayerStat>();
            if (playerStat != null)
            {
                PlayerUltimate.instance.AddMana(10);

                float dam = playerStat.damage * enemyStat.defense;

                //Làm tròn giá trị sát thương
                dam = NumberFomatter.RoundFloatToTwoDecimalPlaces(dam);
                float heal = NumberFomatter.RoundFloatToTwoDecimalPlaces(dam * playerStat.bonusStatAtCurrentLevel.lifeStealPercentBonus);

                enemyStat.TakeDamage(dam);
                UIManager.instance.DisplayDamageText(dropCoinNormalVFX.transform, displayDealtDamTransform, dam);
                playerStat.Healing(heal);
                CameraManager.instance.StartCoroutine(CameraManager.instance.ShakeCamera(3f, 3f, 0.5f));
                BeingAttactk();

                if (enemyStat.CheckIfObjectDead())
                {
                    //Add coin and Play drop coin VFX
                    dropCoinNormalVFX.Play();
                    CurrencyManager.instance.StartCoroutine(CurrencyManager.instance.AddCoins(transform.position, (int)Mathf.Max(1, dam * 0.05f), true, 0f));
                    

                    var playerAttack = other.GetComponentInParent<PlayerAttack>();
                    if (playerAttack != null)
                    {
                        if (playerAttack.attackState == "DoneAttack")
                        {
                            //StartCoroutine(PlayDropCoinEffectWhenPlayerDie());

                            animator.SetBool(isDeadHash, true); // Trigger dead animation
                            if (enemyStat.enemyType == EnemyType.normal)
                            {
                                //CameraManager.instance.StartCoroutine(CameraManager.instance.SetCamWhenTargetDie(false, 3, -8));
                                CameraManager.instance.StartCoroutine(CameraManager.instance.CutSceneAtEnemyWhenEnemyDieByNormallAttack(this, 30));
                            }
                            else
                            {
                                //CameraManager.instance.StartCoroutine(CameraManager.instance.SetCamWhenTargetDie(false, 5, -6));
                                CameraManager.instance.StartCoroutine(CameraManager.instance.CutSceneAtEnemyWhenEnemyDieByNormallAttack(this, 40));
                            }

                            if (MissionsManager._instance.missions != null)
                            {
                                MissionsManager._instance.EnemyKilled();
                            }
                            int randomIndex = Random.Range(0, enemyDieVoice.Length);
                            AudioManager.instance.PlaySFX(enemyDieVoice[randomIndex]);

                            //Destroy(gameObject, 1f);
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
                PlayerUltimate.instance.AddMana(10 * GameManager.instance.multipleScoreForPlayerHit);

                float dam = playerStat.damage * GameManager.instance.multipleScoreForPlayerHit * enemyStat.defense;
                GameManager.instance.multipleScoreForPlayerHit = 1;

                //Làm tròn giá trị sát thương
                dam = NumberFomatter.RoundFloatToTwoDecimalPlaces(dam);
                float heal = NumberFomatter.RoundFloatToTwoDecimalPlaces(dam * playerStat.bonusStatAtCurrentLevel.lifeStealPercentBonus);

                enemyStat.TakeDamage(dam);
                UIManager.instance.DisplayDamageText(dropCoinNormalVFX.transform, displayDealtDamTransform, dam);
                playerStat.Healing(heal);
                BeingAttackSpecial();

                CameraManager.instance.StartCoroutine(CameraManager.instance.ShakeCamera(3f, 3f, 0.5f));

                if (enemyStat.CheckIfObjectDead())
                {
                    //Add coin and Play drop coin VFX
                    dropCoinNormalVFX.Play();
                    CurrencyManager.instance.StartCoroutine(CurrencyManager.instance.AddCoins(transform.position, (int)Mathf.Max(1, dam * 0.05f), true, 0));

                    var playerAttack = other.GetComponentInParent<PlayerAttack>();
                    if (playerAttack != null)
                    {
                        if (playerAttack.attackState == "DoneAttack")
                        {
                            animator.SetBool(isDeadHash, true); // Trigger dead animation

                            if (enemyStat.enemyType == EnemyType.normal)
                            {
                                //CameraManager.instance.StartCoroutine(CameraManager.instance.SetCamWhenTargetDie(false, 3, -8));
                                CameraManager.instance.StartCoroutine(CameraManager.instance.CutSceneAtEnemyWhenEnemyDieBySpecialAttack(this, 24, 30, 0.35f));
                            }
                            else
                            {
                                //CameraManager.instance.StartCoroutine(CameraManager.instance.SetCamWhenTargetDie(false, 5, -6));
                                CameraManager.instance.StartCoroutine(CameraManager.instance.CutSceneAtEnemyWhenEnemyDieBySpecialAttack(this, 30, 40, 0.3f));
                            }

                            if (MissionsManager._instance.missions != null)
                            {
                                MissionsManager._instance.EnemyKilled();
                            }
                            int randomIndex = Random.Range(0, enemyDieVoice.Length);
                            AudioManager.instance.PlaySFX(enemyDieVoice[randomIndex]);

                        }
                    }
                }
                else
                {
                    if (enemyStat.enemyType == EnemyType.normal)
                    {
                        CameraManager.instance.StartCoroutine(CameraManager.instance.SetCamForSpecialAttack(0.35f, 24f));
                    }
                    else
                    {
                        CameraManager.instance.StartCoroutine(CameraManager.instance.SetCamForSpecialAttack(0.3f, 30f));
                    }

                    int randomIndex = Random.Range(0, enemyHitVoice.Length);
                    AudioManager.instance.PlaySFX(enemyHitVoice[randomIndex]);
                }

                if (!animator.GetBool(isDeadHash))
                {
                    animator.SetTrigger(getHitHash); // Trigger the get hit animation
                }
            }
        }
    }

    public IEnumerator PlayDropCoinEffectWhenEnemyDie()
    {
        yield return new WaitForSeconds(0.5f);
        dropCoinSpecialVFX.Play();
        AudioManager.instance.PlaySFX("DropCoinWhenEnemyDie");
        CurrencyManager.instance.StartCoroutine(CurrencyManager.instance.AddCoins(transform.position, enemyStat.coinReward, true, 0f));
        if (enemyStat.crystalReward > 0)
        {
            dropCrystalVFX.Play();
            CurrencyManager.instance.StartCoroutine(CurrencyManager.instance.AddCrystals(transform.position + new Vector3(0, 0.25f, 0), enemyStat.crystalReward, true, 0.25f));
        }
        if (enemyStat.starReward > 0)
        {
            CurrencyManager.instance.StartCoroutine(CurrencyManager.instance.AddStars(transform.position + new Vector3(0, 0.5f, 0), enemyStat.starReward, true, 0.5f));
            yield return new WaitForSeconds(0.2f);
            dropStarVFX.Play();
        }
    }

    public void BeingAttactk()
    {
        //int randomIndex = Random.Range(0, beingHitText.Length);
        hitImpact.Play();
        bloodSplash.Play();

        //beingHitText[randomIndex].Play();
    }

    public void BeingAttackSpecial()
    {
        //int randomIndex = Random.Range(0, beingHitText.Length);
        hitImpactSpecial.Play();
        bloodSplash.Play();
        //beingHitText[randomIndex].Play();
    }

    //public void DisplayDamageText(float damage)
    //{
    //    string damageText = "-" +NumberFomatter.FormatFloatToString(damage, 2);
    //    particleDamagePrefab.UpdateText(damageText); // Update the text in the prefab
    //    damageDisplay.Play(); // Play the damage display effect
    //}
}
