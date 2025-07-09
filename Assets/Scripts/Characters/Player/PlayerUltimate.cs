using UnityEngine;

public class PlayerUltimate : MonoBehaviour
{
    public static PlayerUltimate instance; // Singleton instance of PlayerUltimate
    public Transform playerTransform;

    private int ultimateHash;

    [Header("Stat")]
    public float basicDamagePlayer; // Damage dealt by the basic attack
    public float basicMaxHealthPlayer;
    public float basicCurrentHealthPlayer; // Current health of the player
    public float ultimateDamage; // Damage dealt by the ultimate ability

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Assign the instance to this script
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        ultimateHash = Animator.StringToHash("Ultimate"); // Get the hash for the ultimate animation
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindPlayerTransform()
    { 
        playerTransform = GameObject.FindWithTag("Player").transform;
        basicDamagePlayer = playerTransform.GetComponent<PlayerStat>().damage;
        basicMaxHealthPlayer = playerTransform.GetComponent<PlayerStat>().maxHealth;
        AddUltimateToUltiButton(playerTransform.GetComponent<PlayerStat>().id);
    }

    public void AddUltimateToUltiButton(int idPlayer)
    { 
        UIManager.instance.ultimateButton.onClick.RemoveAllListeners();

        if (idPlayer == 0)
        {
            UIManager.instance.ultimateButton.onClick.AddListener(() => Player1Ultimate());
        }
    }

    public void Player1Ultimate()
    { 
        playerTransform.GetComponent<Player>().animator.SetTrigger(ultimateHash); // Trigger the ultimate animation
        playerTransform.GetComponent<PlayerAttack>().playerStat.Healing(basicMaxHealthPlayer * 30 / 100);
    }
}
