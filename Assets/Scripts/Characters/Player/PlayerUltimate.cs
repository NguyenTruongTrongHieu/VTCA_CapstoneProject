using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUltimate : MonoBehaviour
{
    public static PlayerUltimate instance; // Singleton instance of PlayerUltimate
    public Transform playerTransform;

    private int ultimateHash;

    [Header("Mana")]
    public float mana;
    public float maxMana;

    [Header("Basic player's stat")]
    public float basicDamagePlayer; // Damage dealt by the basic attack
    public float basicMaxHealthPlayer;
    public float basicCurrentHealthPlayer; // Current health of the player
    public float basicLifeStealPlayer; // Lifesteal percentage of the player

    [Header("Ultimate's stat")]
    public float lifeStealPercent; // Lifesteal percentage during the ultimate ability
    public float ultimateDamage; // Damage dealt by the ultimate ability

    [Header("Check if ultimate is valid")]
    public int totalRound;
    public bool isUltimateValid = false;

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
        GetPlayerTransform(SaveLoadManager.instance.currentPlayerName, SaveLoadManager.instance.currentLevelOfCurrentPlayer); // Get the player transform for Player1
        ultimateHash = Animator.StringToHash("Ultimate"); // Get the hash for the ultimate animation
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GetPlayerTransform(string playerName, int currentLevelOfPlayer)
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

                playerStat.bonusStatAtCurrentLevel = playerStat.bonusStatsLevel[currentLevelOfPlayer - 1]; // Set the bonus stats for the player at the current level
                playerStat.SetUpStatAndSlider();

                SetUpBaseStatForPlayer(); // Set up the base stats for the player

                player.GetComponent<Player>().ReturnStartPos();
                CameraManager.instance.SetTargetForCam(player.transform);//call when change player
                player.GetComponent<Player>().SetUpBehaviourTree();
                //AddUltimateToUltiButton(playerTransform.GetComponent<PlayerStat>().id);
            }
        }
    }

    public void TurnOffAllPlayersTransform()
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

            player.SetActive(false); // Deactivate all player objects
        }
    }

    public void SetUpBaseStatForPlayer()
    {
        basicDamagePlayer = playerTransform.GetComponent<PlayerStat>().damage; // Get the player's damage
        basicMaxHealthPlayer = playerTransform.GetComponent<PlayerStat>().maxHealth; // Get the player's max health
        basicLifeStealPlayer = playerTransform.GetComponent<PlayerStat>().bonusStatAtCurrentLevel.lifeStealPercentBonus;
    }

    public void ResetPlayer()
    {
        if (playerTransform == null)
        { 
            Debug.LogWarning("Player transform is not set. Cannot reset player.");
            return;
        }

        var playerStat = playerTransform.GetComponent<PlayerStat>();
        playerStat.SetUpStatAndSlider();
        playerTransform.GetComponent<PlayerAttack>().ResetAnimState();
        CameraManager.instance.ResetCamForPlayer();
        playerTransform.GetComponent<Player>().ReturnStartPos();
        playerTransform.GetComponent<Player>().SetUpBehaviourTree();
        ResetUltiAndMana(false); // Reset ultimate and mana when resetting the player

    }

    public void FindPlayerTransform()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        basicDamagePlayer = playerTransform.GetComponent<PlayerStat>().damage;
        basicMaxHealthPlayer = playerTransform.GetComponent<PlayerStat>().maxHealth;
        //AddUltimateToUltiButton(playerTransform.GetComponent<PlayerStat>().id);
    }

    public void AddUltimateToUltiButton(int idPlayer)
    {
        UIManager.instance.ultimateButton.onClick.RemoveAllListeners() ;

        if (idPlayer == 0)
        {
            UIManager.instance.ultimateButton.onClick.AddListener(() => Player1Ultimate());
        }
        else if (idPlayer == 1)
        {
            UIManager.instance.ultimateButton.onClick.AddListener(() => Player2Ultimate());
        }
        else if (idPlayer == 2)
        {
            UIManager.instance.ultimateButton.onClick.AddListener(() => Player3Ultimate());
        }
    }

    public void ResetUltiAndMana(bool isHavingSliderAnim)
    {
        mana = 0;
        UIManager.instance.ultimateButton.gameObject.SetActive(false);
        if (isHavingSliderAnim)
        {
            UIManager.instance.StartCoroutine(UIManager.instance.AppearManaSlider(0.2f)); // Update the mana slider and ultimate button in the UI
        }
        else
        {
            UIManager.instance.manaSlider.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(UIManager.instance.manaSlider.GetComponent<RectTransform>().anchoredPosition.x, 
                UIManager.instance.startPosYManaSlider);
            UIManager.instance.manaSlider.gameObject.SetActive(true); // Show the mana slider when resetting
        }

        //Reset slider mana in uimnager
        UIManager.instance.manaSlider.value = 0; // Reset the mana slider to 0
    }

    public void AddMana(float amount)
    {
        if (mana >= maxMana)
            return;

        mana = Mathf.Min(mana + amount, maxMana);
        UIManager.instance.StartCoroutine(UIManager.instance.IncreaseManaSliderValue(0.1f, mana)); // Update the mana slider in the UI

        if (mana >= maxMana)
        {
            //UIManager.instance.ultimateButton.gameObject.SetActive(true); // Enable the ultimate button when mana is full
            //UIManager.instance.manaSlider.gameObject.SetActive(false); // Show the mana slider when mana is full
            UIManager.instance.StartCoroutine(UIManager.instance.ChangeManaSliderAndUltimateButton());
        }
    }

    public void Player1Ultimate()
    {
        isUltimateValid = true; // Set ultimate as valid
        playerTransform.GetComponent<Player>().animator.SetTrigger(ultimateHash); // Trigger the ultimate animation

        lifeStealPercent = basicLifeStealPlayer + 0.1f;
        playerTransform.GetComponent<PlayerStat>().bonusStatAtCurrentLevel.lifeStealPercentBonus = lifeStealPercent; 

        totalRound = 3; // Set the total rounds for the ultimate ability

        StartCoroutine(PlayUltiVfx(2f)); // Start the coroutine to play ultimate visual effects
        playerTransform.GetComponent<PlayerAttack>().PlayUltiVFX2();
        StartCoroutine(Player1UltimateCoroutine()); // Start the coroutine for Player 1's ultimate

        ResetUltiAndMana(true);
    }

    public IEnumerator Player1UltimateCoroutine()
    {
        while (totalRound > 0)
        {
            yield return null;
        }
        playerTransform.GetComponent<PlayerAttack>().playerStat.bonusStatAtCurrentLevel.lifeStealPercentBonus = basicLifeStealPlayer;
        playerTransform.GetComponent<PlayerAttack>().StopUltiVFX2();
        isUltimateValid = false; // Reset ultimate validity after use
    }
    public IEnumerator PlayUltiVfx(float waitTime)
    { 
        playerTransform.GetComponent<PlayerAttack>().PlayUltiVFX(); // Trigger the ultimate animation
        yield return new WaitForSeconds(waitTime); // Wait for the specified time
        playerTransform.GetComponent<PlayerAttack>().StopUltiVFX(); // Stop the ultimate visual effect
    }
    public void Player2Ultimate()
    {
        isUltimateValid = true; // Set ultimate as valid
        playerTransform.GetComponent<Player>().animator.SetTrigger(ultimateHash); // Trigger the ultimate animation

        ultimateDamage = basicDamagePlayer + (basicDamagePlayer * 0.15f); 
        playerTransform.GetComponent<PlayerAttack>().playerStat.damage = ultimateDamage;

        totalRound = 3; // Set the total rounds for the ultimate ability

        StartCoroutine(Player2UltimateCoroutine()); // Start the coroutine for Player 2's ultimate

        StartCoroutine(PlayUltiVfx(0.75f));
        playerTransform.GetComponent<PlayerAttack>().PlayUltiVFX2(); // Play the ultimate visual effect

        ResetUltiAndMana(true);
    }

    public IEnumerator Player2UltimateCoroutine()
    {
        while (totalRound > 0)
        {
            yield return null;
        }
        playerTransform.GetComponent<PlayerAttack>().playerStat.damage = basicDamagePlayer;
        playerTransform.GetComponent<PlayerAttack>().StopUltiVFX2();
        isUltimateValid = false; // Reset ultimate validity after use
    }

    public void Player3Ultimate()
    { 
        isUltimateValid = true;
        playerTransform.GetComponent<Player>().animator.SetTrigger(ultimateHash);

        StartCoroutine(SpawnDebuffTakeDam());

        isUltimateValid = false; // Reset ultimate validity after use

        StartCoroutine(PlayUltiVfx(1.5f));

        ResetUltiAndMana(true);
    }

    public IEnumerator SpawnDebuffTakeDam()
    {
        List<int> usedCell = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            int foodIndex;
            do
            {
                foodIndex = Random.Range(0, GameBoard.Instance.foodList.Count);
                yield return null;
            }
            while (usedCell.Contains(foodIndex) || GameBoard.Instance.foodList[foodIndex].foodType == FoodType.Special);
            
            usedCell.Add(foodIndex);
            Food oldFood = GameBoard.Instance.DeleteFoodAtPos(GameBoard.Instance.foodList[foodIndex].xIndex, 
                GameBoard.Instance.foodList[foodIndex].yIndex);
             yield return oldFood.StartCoroutine(oldFood.ZoomOut(0.25f, 0));

            GameObject foodObject = Instantiate(GameBoard.Instance.debuffTakeDamSpecialFoodPrefab, 
               oldFood.transform.position, Quaternion.identity, GameBoard.Instance.foodParent);
            foodObject.transform.localScale = Vector3.zero;
            Food food = foodObject.GetComponent<Food>();
            
            yield return food.StartCoroutine(food.ReturnOriginalScale(0.25f));

            GameBoard.Instance.AddFoodAtPos(oldFood.xIndex, oldFood.yIndex, food);
            Destroy(oldFood.gameObject); // Destroy the old food object
        }
    }
}
