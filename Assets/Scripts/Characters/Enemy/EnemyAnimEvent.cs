using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour
{
    [Header("Hit")]
    public Collider firstHitCollider; // Collider for the weapon, used to detect hits
    public Collider secondHitCollider;
    //public Collider specialHitWeaponCollider; // Collider for special hits, used to detect hits from special attacks

    [Header("Sound")]
    public string attackSound;

    public void TurnOnFirstHitCollider()
    {
        firstHitCollider.enabled = true; // Enable the weapon collider to detect hits
        if (this.transform.parent == LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform)
        { 
            AudioManager.instance.PlaySFX(attackSound);
        }
    }

    public void TurnOffFirstHitCollider()
    {
        firstHitCollider.enabled= false;
    }

    public void TurnOnSecondHitCollider()
    { 
        secondHitCollider.enabled = true;
        if (this.transform.parent == LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform)
        {
            AudioManager.instance.PlaySFX(attackSound);
        }
    }

    public void TurnOffSecondHitCollider()
    { 
        secondHitCollider.enabled = false;
    }
}
