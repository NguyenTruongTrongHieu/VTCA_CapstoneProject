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

            LevelManager.instance.SpawnEnemiesAtCurrentLevel();

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
        GetPlayerTransform(SaveLoadManager.instance.currentPlayerName); // Get the player transform for Player1
        ultimateHash = Animator.StringToHash("Ultimate"); // Get the hash for the ultimate animation
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPlayerTransform(string playerName)
    { 
        int childCount = transform.childCount; // Get the number of child objects
        for (int i = 0; i < childCount; i++)
        { 
            var player = transform.GetChild(i).gameObject; // Get each child object
            PlayerStat playerStat;

            if (!player.TryGetComponent<PlayerStat>(out playerStat))
            {
                continue;
            }

            if (player.GetComponent<PlayerStat>().name == playerName)
            {
                player.SetActive(true);
                playerTransform = player.transform;
                playerStat.SetUpStatAndSlider(); 
                basicDamagePlayer = playerStat.damage; // Get the player's damage
                basicMaxHealthPlayer = playerStat.maxHealth; // Get the player's max health
                player.GetComponent<Player>().ReturnStartPos();
                CameraManager.instance.SetTargetForCam(player.transform);//call when change player
                player.GetComponent<Player>().SetUpBehaviourTree();
            }
        }
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
