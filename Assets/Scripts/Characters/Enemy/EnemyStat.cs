using System.Runtime.CompilerServices;
using UnityEditor.ShaderGraph.Internal;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupHPAndUpdateSlider();
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
