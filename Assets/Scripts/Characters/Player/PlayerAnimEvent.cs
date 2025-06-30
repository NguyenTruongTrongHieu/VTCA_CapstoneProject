using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    [Header("Weapon")]
    public Collider weaponCollider; // Collider for the weapon, used to detect hits
    public Collider specialHitWeaponCollider; // Collider for special hits, used to detect hits from special attacks

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
}
