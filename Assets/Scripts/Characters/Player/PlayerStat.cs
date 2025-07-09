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
    public int id;
    public string name; // Player's name
    public PlayerClass playerClass; // Player's class

    [Header("Percent bonus")]
    public float healthPercentBonus; // Bonus health percentage
    public float damagePercentBonus; // Bonus damage percentage

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damage = GameManager.instance.basicDamage + GameManager.instance.basicDamage * damagePercentBonus;
        maxHealth = GameManager.instance.basicHealth + GameManager.instance.basicHealth * healthPercentBonus;
        SetupHPSlider();

        PlayerUltimate.instance.FindPlayerTransform(); // Find the player's transform at the start
    }

    // Update is called once per frame
    void Update()
    {

    }
}
