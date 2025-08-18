using System.Runtime.CompilerServices;
using UnityEngine;

public enum EnemyType
{
    normal,
    boss,
}

public class EnemyStat : Stats
{
    [Header("Information")]
    public int id;
    public string name; // Enemy's name
    public EnemyType enemyType; // Enemy's type

    [Header("Bonus Stat")]
    [SerializeField] private float basicDefense = 1f;
    public float defense = 1f;

    [Header("Reward")]
    public int coinReward = 0; // Amount of coins rewarded when the enemy is defeated
    public int diamondReward = 0; // Amount of diamonds rewarded when the enemy is defeated

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCurrentHealth();
        SetUpDefense();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpDefense()
    {
        defense = basicDefense;
    }
}
