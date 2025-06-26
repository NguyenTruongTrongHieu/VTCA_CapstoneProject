using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("Player's stat")]
    public float maxHealth; // Player's maximum health
    public float health; // Player's health
    public float damage;

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

    public void TakeDamage(float dam)
    {
        health = Mathf.Max(0, health - dam);
    }

    public void Healing(float heal)
    { 
        health = Mathf.Min(health + heal, maxHealth);
    }
}
