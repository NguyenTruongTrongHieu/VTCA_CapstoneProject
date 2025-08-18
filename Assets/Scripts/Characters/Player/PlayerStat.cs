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
    [Header("Information")]
    public int id;
    public string name; // Player's name
    public PlayerClass playerClass; // Player's class
    public bool isNormalSkin;

    [Header("Percent bonus")]
    public BonusStatForPlayer bonusStatAtCurrentLevel;
    public BonusStatForPlayer[] bonusStatsLevel;  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUpDamAndHealth()
    {
        damage = GameManager.instance.basicDamage + GameManager.instance.basicDamage * bonusStatAtCurrentLevel.damagePercentBonus;
        maxHealth = GameManager.instance.basicHealth + GameManager.instance.basicHealth * bonusStatAtCurrentLevel.healthPercentBonus;
    }

    public void SetUpStatAndSlider()
    { 
        SetUpDamAndHealth();
        SetCurrentHealth();
    }
}

[System.Serializable]
public class BonusStatForPlayer
{
    public float healthPercentBonus; // Bonus health percentage
    public float damagePercentBonus; // Bonus damage percentage
    public float lifeStealPercentBonus; // Bonus lifesteal percentage
    public int coinCost; // Cost in coins to buy this level
    public int starCost; // Cost in stars to buy this level
    public int crystalCost; // Cost in crystals to buy this level
}
