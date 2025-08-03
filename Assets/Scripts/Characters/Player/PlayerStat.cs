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
        SetupHPAndUpdateSlider();
    }
}

[System.Serializable]
public class BonusStatForPlayer
{
    public float healthPercentBonus; // Bonus health percentage
    public float damagePercentBonus; // Bonus damage percentage
    public float lifeStealPercentBonus; // Bonus lifesteal percentage
}
