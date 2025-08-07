using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    [Header("COLLIDER Weapon or normal attack")]
    public Collider weaponCollider; // Collider for the weapon, used to detect hits
    public Collider specialHitWeaponCollider; // Collider for special hits, used to detect hits from special attacks

    [Header("VFX")]
    public ParticleSystem hitVFX;
    public ParticleSystem specialHitVFX;
    public ParticleSystem specialHitVFX2;

    [Header("Light")]
    public Light specialHitVFXLight;


    public void TurnOnWeaponColider()
    { 
        weaponCollider.enabled = true; // Enable the weapon collider to detect hits
    }

    public void TurnOffWeaponColider()
    {
        weaponCollider.enabled = false; // Disable the weapon collider to stop detecting hits
    }

    public void TurnOnSpecialHitWeaponCollider()
    {
        specialHitWeaponCollider.enabled = true; // Enable the special hit weapon collider to detect hits from special attacks
    }

    public void TurnOffSpecialHitWeaponCollider()
    {
        specialHitWeaponCollider.enabled = false; // Disable the special hit weapon collider to stop detecting hits from special attacks
    }

    public void TurnCurrentTurnToNone()
    { 
        GameManager.instance.currentTurn = "None"; // Set the current turn to None, indicating no active turn
    }

    public void TurnCurrentTurnToPlayer()
    {
        GameManager.instance.currentTurn = "Player"; // Set the current turn to Player, indicating it's the player's turn
    }

    public void TurnOnSpecialHitVFX()
    { 
        specialHitVFX.Play(); // Play the special hit visual effect
    }

    public void TurnOnHitVFX()
    { 
        hitVFX.Play(); // Play the normal hit visual effect
    }

    public void TurnOffHitVFX()
    {
        hitVFX.Stop(); // Stop the normal hit visual effect
    }

    public void TurnOffSpecialHitVFX()
    {
        specialHitVFX.Stop(); // Stop the special hit visual effect
    }

    public void TurnOnSpecialHitLightVFX()
    { 
        specialHitVFXLight.gameObject.SetActive(true); // Enable the light for the special hit visual effect
    }

    public void TurnOffSpecialHitLightVFX()
    {
        specialHitVFXLight.gameObject.SetActive(false); // Disable the light for the special hit visual effect
    }

    public void TurnOnSpecialHitVFX2()
    { 
        specialHitVFX2.Play(); // Play the second special hit visual effect
    }

    public void TurnOnAndSetPosAtEnemyForHitVFX()
    {
        hitVFX.gameObject.transform.position = LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform.position 
            + new Vector3(0, 0, 0);
        hitVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);// Stop the hit VFX to reset it before playing again
        hitVFX.Play();
    }
}
