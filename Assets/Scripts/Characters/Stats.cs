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
}
