using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [Header("Basic stat")]
    public float maxHealth; // Player's maximum health
    public float currentHealth; // Player's health
    public float damage;

    [Header("Slider HP")]
    public Slider hpSlider; // Slider to display health

    public void TakeDamage(float dam)
    {
        currentHealth = Mathf.Max(0, currentHealth - dam);
    }

    public void Healing(float heal)
    {
        currentHealth = Mathf.Min(currentHealth + heal, maxHealth);
    }

    public virtual void SetupHPSlider()
    {
        currentHealth = maxHealth; // Initialize current health to max health
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }

    public virtual bool CheckIfObjectDead()
    {
        bool result = false;
        if (currentHealth <= 0)
        {
            result = true; // Object is dead
            currentHealth = 0; // Ensure health does not go below zero
        }

        return result;
    }
}
