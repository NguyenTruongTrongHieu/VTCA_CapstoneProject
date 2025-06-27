using UnityEngine;

public enum PlayerClass
{
    adc,
    tank,
    support,
    mage,
    assassin,
    warrior,
}

public class PlayerStat : Stats
{
    [Header("Ulti")]
    public float mana;
    public float maxMana;

    [Header("Information")]
    public string name; // Player's name
    public PlayerClass playerClass; // Player's class

    [Header("Percent bonus")]
    public float healthPercentBonus; // Bonus health percentage
    public float damagePercentBonus; // Bonus damage percentage

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
