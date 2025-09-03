using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [Header("Basic stat")]
    public float maxHealth; // Player's maximum health
    public float currentHealth; // Player's health
    public float damage;

    [Header("Slider HP")]
    public HPSlider hpSlider; // Slider to display health

    public void TakeDamage(float dam)
    {
        currentHealth = Mathf.Max(0, currentHealth - dam);
        if (hpSlider != null && dam > 0)
        {
            hpSlider.MinusValue(currentHealth); // Update the HP slider when taking damage
        }
    }

    public void Healing(float heal)
    {
        currentHealth = Mathf.Min(currentHealth + heal, maxHealth);
        if (hpSlider != null && heal > 0)
        {
            hpSlider.PlusValue(currentHealth, heal); // Update the HP slider when healing
            AudioManager.instance.PlaySFX("Healing");
        }
    }

    public void SetCurrentHealth()
    {
        currentHealth = maxHealth; // Set current health to maximum health
    }

    public virtual void SetHPSlider(bool isPlayer)
    {
        if (isPlayer)
        { 
            hpSlider = UIManager.instance.playerHPSlider.GetComponent<HPSlider>(); // Assign the player's HP slider
            hpSlider.SetSLiderAtStart(PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().name, maxHealth);
        }
        else
        {
            hpSlider = UIManager.instance.enemyHPSlider.GetComponent<HPSlider>(); // Assign the enemy's HP slider
            hpSlider.SetSLiderAtStart(LevelManager.instance.currentLevel.enemiesAtLevel
                [GameManager.instance.currentEnemyIndex].GetComponent<EnemyStat>().name, maxHealth); // Use the object's name for the slider
        }
        UIManager.instance.StartCoroutine(UIManager.instance.AppearHPSlider(isPlayer, 0.3f));
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
