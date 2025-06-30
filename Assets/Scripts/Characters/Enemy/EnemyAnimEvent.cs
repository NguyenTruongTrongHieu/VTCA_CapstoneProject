using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour
{
    [Header("Hit")]
    public Collider firstHitCollider; // Collider for the weapon, used to detect hits
    public Collider secondHitCollider;
    //public Collider specialHitWeaponCollider; // Collider for special hits, used to detect hits from special attacks

    public void TurnOnFirstHitCollider()
    {
        firstHitCollider.enabled = true; // Enable the weapon collider to detect hits
    }

    public void TurnOffFirstHitCollider()
    {
        firstHitCollider.enabled= false;
    }

    public void TurnOnSecondHitCollider()
    { 
        secondHitCollider.enabled = true;
    }

    public void TurnOffSecondHitCollider()
    { 
        secondHitCollider.enabled = false;
    }
}
