using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerStat playerStat;

    public Animator animator;
    public List<string> attackAnimations; // List of animation names to play
    private List<int> attackHashes = new List<int>();//Trigger
    private int specialAttackHash;//Trigger
    private int doneAttackHash; // Trigger for done attack

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        specialAttackHash = Animator.StringToHash("specialAttack");
        doneAttackHash = Animator.StringToHash("DoneAttack");
        for (int i = 0; i < attackAnimations.Count; i++)
        {
            attackHashes.Add(Animator.StringToHash(attackAnimations[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        { 
            animator.SetTrigger(specialAttackHash);
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        { 
            StartCoroutine(PlayAttackSequence(5));
        }
    }

    public IEnumerator PlayAttackSequence(int totalHits)
    {
        for (int i = 0; i < totalHits; i++)
        {
            Debug.Log($"Playing attack {i + 1}/{totalHits}");
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

        //yield return new WaitForSeconds(0.5f); // Thời gian nghỉ sau khi ra hết đòn
        animator.SetTrigger(doneAttackHash); // Kết thúc chuỗi tấn công
    }
}
