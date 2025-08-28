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
    public int crystalReward = 0; // Amount of diamonds rewarded when the enemy is defeated
    public int starReward = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetRandomCrystalAndStarReward();
        SetCurrentHealth();
        SetUpDefense();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRandomCrystalAndStarReward()
    { 
        //Set crystal reward with a 1% chance
        float randomValue = Random.Range(0, 101); // Random value between 0 and 100
        if (randomValue <= 90f) // 1% chance
        {
            crystalReward = 1;
        }
        else
        {
            crystalReward = 0; // No crystals rewarded
        }

        //Set star reward with a 30% chance
        randomValue = Random.Range(0, 101); // Random value between 0 and 100
        if (randomValue <= 100f) // 30% chance
        {
            starReward = 1;
        }
        else
        {
            starReward = 0; // No stars rewarded
        }
    }

    public void SetUpDefense()
    {
        defense = basicDefense;
    }
}
