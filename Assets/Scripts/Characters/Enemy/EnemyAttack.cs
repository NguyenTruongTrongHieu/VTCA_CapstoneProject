using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public EnemyStat enemyStat;

    private bool isAttacking = false;

    public Animator animator;
    public List<string> attackAnimations; // List of animation names to play
    private List<int> attackHashes = new List<int>();//Trigger
    private int doneAttackHash; // Trigger for done attack

    private int specialAttackHash;//Trigger, maybe use in the future

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isAttacking = false;
        doneAttackHash = Animator.StringToHash("DoneAttack");
        for (int i = 0; i < attackAnimations.Count; i++)
        {
            attackHashes.Add(Animator.StringToHash(attackAnimations[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState == GameState.Playing && GameManager.instance.currentTurn == "Enemy" && !isAttacking)
        { 
            isAttacking = true;
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
            yield return new WaitForSeconds(1f); // Thời gian cho đòn tấn công đặc biệt
        }

        // Gọi hàm DoneAttack sau khi kết thúc chuỗi tấn công
        StartCoroutine(DoneAttack(false));
    }

    public IEnumerator DoneAttack(bool isPlayerDie)
    {
        yield return new WaitForSeconds(0.5f); // Thời gian nghỉ sau khi ra hết đòn
        animator.SetTrigger(doneAttackHash); // Kết thúc chuỗi tấn công

        //Check if current enemy die
        if (isPlayerDie)
        {
            GameManager.instance.currentTurn = "Lose";
        }
        else
        {
            GameManager.instance.currentTurn = "Player";
        }

        isAttacking = false; // Đặt lại trạng thái tấn công
    }
}
