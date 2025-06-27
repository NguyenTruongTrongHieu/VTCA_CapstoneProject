using UnityEngine;

public enum EnemyType
{
    normal,
    boss,
}

public class EnemyStat : Stats
{
    [Header("Information")]
    public string name; // Enemy's name
    public EnemyType enemyType; // Enemy's type

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
