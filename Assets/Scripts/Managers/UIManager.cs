using CartoonFX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class CharacterButton
{
    public int characterID;
    public string characterName;
    public GameObject changeCharacterObject;
    public Text ownedText;
    public Text buyText;
    public Button chosenButton;
    public Image chosenImage;
    public Image lockImage;
    public bool isOwned = false;

    public CharacterButton(int characterID, string characterName, GameObject changeCharacterObject)
    {
        this.characterID = characterID;
        this.characterName = characterName;
        this.changeCharacterObject = changeCharacterObject;
        isOwned = false;
    }

    public CharacterButton(int id, string name, Button button)
    {
        characterID = id;
        characterName = name;
        chosenButton = button;
        isOwned = false;
    }

    public CharacterButton(int id, string name, GameObject changeObject, Text ownedText, Text buyText, Button button, Image image)
    {
        characterID = id;
        characterName = name;
        changeCharacterObject = changeObject;
        this.ownedText = ownedText;
        this.buyText = buyText;
        chosenButton = button;
        chosenImage = image;
        isOwned = false;
    }
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TabsManager tabsManager;


    [Header("Currency")]
    public GameObject coinPanel;
    public Text coinText;
    public GameObject crystalPanel;
    public Text crystalText;
    public GameObject starPanel;
    public Text starText;

    [Header("Setting")]
    public GameObject settingPanel;
    public Button returnHomeButton;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("Main menu")]
    public Button playButton;
    public Button upgradeDamButton;
    public Text damageLevelText;
    public Text basicDamageText;
    public Text costToUpgradeDamText;

    public Button upgradeHealthButton;
    public Text healthLevelText;
    public Text basicHealthText;
    public Text costToUpgradeHealthText;

    public GameObject mainMenuPanel;
    private Vector2 originaloffsetMinMenuPanel;
    private Vector2 originaloffsetMaxMenuPanel;

    [Space]
    public Text currentLevelDisplay;
    public Image skullImage;

    [Header("Character And Skin")]
    public GameObject characterPanel;
    public GameObject[] skinPanels;
    public Text levelText;
    public Text damTextInUI;
    public Text healthTextInUI;
    public Text ultiDescriptionText;
    public Text ultiStatText;
    public Button buyCharacterButton;
    public Button upgradeCharacterButton;

    [Space]
    public GameObject buyAndUpgradeCharacterPanel;

    public GameObject BuyCharacterPanel;
    public Text titleBuyCharacterText;
    public Button useCoinToBuyCharacterButton;
    public Button useCrystalToBuyCharacterButton;
    public Button useStarToBuyCharacterButton;
    public Text useCoinToBuyCharacterText;
    public Text useCrystalToBuyCharacterText;
    public Text useStarToBuyCharacterText;
    public GameObject quitBuyCharacterPanelButton;

    [Space]
    public GameObject UpgradeCharacterPanel;
    public Text titleUpgradeCharacterText;
    public Text upgradeDamStatText;
    public Text upgradeDamStatAtNewLevelText;
    public Text upgradeHealthStatText;
    public Text upgradeHealthStatAtNewLevelText;
    public Text coinToUpgradeText;
    public Text starToUpgradeText;
    public GameObject quitUpgradeCharacterPanelButton;

    [Space]
    public GameObject WarningNotEnoughCostPanel;
    public Text warningNotEnoughCostText;

    [Space]
    public CharacterButton[] characterButtons; // Array of character buttons
    public CharacterButton currentChosenButton;

    [Header("In Game")]
    public GameObject inGamePanel;
    public GameObject gameBoard;
    public GameObject disableMatching;
    public GameObject ultimateButtonAndEffectObject;
    public Button ultimateButton;
    public Slider manaSlider;
    public float startPosYManaSlider;
    public Text currentStageProgressionDisplay;

    [Header("HUD")]
    public Slider playerHPSlider;
    public Slider enemyHPSlider;
    public GameObject progressPanel;
    public GameObject[] gameProgress;
    public List<GameObject> usedProgress = new List<GameObject>();

    private Vector2 startPlayerHPSliderPosition;
    private Vector2 startEnemyHPSliderPosition;
    private Vector2 endPlayerHPSliderPosition;
    private Vector2 endEnemyHPSliderPosition;
    private Vector2 startProgressPanelPosition;
    private Vector2 endProgressPanelPosition;


    [Header("Game Over")]
    public GameObject parentObject;
    public GameObject gameOverPanel;
    public GameObject rewardPanel;
    public Image gameOverPanelImage;
    public Image resultImage;
    public Sprite winPanelSprite;
    public Sprite losePanelSprite;
    public Sprite winResultSprite;
    public Sprite loseResultSprite;
    public Sprite winButtonSprite;
    public Sprite loseButtonSprite;
    public Text rewardCoinText;
    public Button returnMenuButton;

    [Header("DamText Prefabs")]
    public GameObject damageText;
    [Tooltip("The width of the character in the damage text prefab, used for positioning the text correctly")]
    public float characterWidthDamText;

    [Header("CoinText prefabs")]
    public GameObject coinTextPrefab;
    public GameObject crystalTextPrefab;
    public GameObject starTextPrefab;
    public float characterWidthCoinText;


    [Header("Missions")]
    public Text[] missionsDescriptionTexts;
    public GameObject[] missionsCompletedClaimButtons;
    public Image[] missionCompletedImage;
    public Slider[] missionsProgressSliders;
    public Text[] missionsProgressTexts;
    public Slider chestSlider;
    public Text chestProgressText;
    public Text resetMissionTimer;
    public ChestManager chestManager;

    [Header("ChestBox")]
    public GameObject openChestBoxPanel;
    public GameObject itemParent;
    public GameObject currencyParent;
    public GameObject rewardTargetPos;

    [Space]
    public Image chestBoxImage;
    public Sprite chestBoxClosedSprite;
    public Sprite chestBoxOpenedSprite;
    public Button claimRewardButton;

    [Space]
    public ParticleSystem lightChestBoxVFX;
    public ParticleSystem smokeChestBoxVFX;

    [Space]
    public GameObject crystalBonusPrefab;
    public GameObject starBonusPrefab;
    public GameObject coinBonusPrefab;
    public GameObject itemBonusPrefab;

    [Space]
    [SerializeField] private int pressCount;
    private bool isShowRewardRunning;
    private List<Item> itemRewardsInChestBox = new List<Item>();//used as item class
    private int crystalsInChestBox;
    private int coinsInChestBox;
    private int starsInChestBox;
    private bool isClaimCrystal = false;
    private bool isClaimCoin = false;
    private bool isClaimStar = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Missions 
        Debug.Log("How many Missions des: " + missionsDescriptionTexts.Length.ToString());

        //Turn on music
        AudioManager.instance.PlayMusic("BackgroundMusic", true);


        // Set up the main menu
        if (LevelManager.instance.currentLevel.index != 0)
        {
            SetUITextForUpgradeDamButton();
            SetUITextForUpgradeHealthButton();
            UpdateColorTextForUpgradeCost();
            UpdateCoinText();
            UpdateCrystalText();
            UpdateStarText();
            SetUpMissionsDescription();
        }

        originaloffsetMinMenuPanel = mainMenuPanel.GetComponent<RectTransform>().offsetMin;
        originaloffsetMaxMenuPanel = mainMenuPanel.GetComponent<RectTransform>().offsetMax;

        ShowMainMenuPanel();

        startPosYManaSlider = manaSlider.GetComponent<RectTransform>().anchoredPosition.y;

        //Set start and end pos for HUD
        startPlayerHPSliderPosition = playerHPSlider.GetComponent<RectTransform>().anchoredPosition;
        startEnemyHPSliderPosition = enemyHPSlider.GetComponent<RectTransform>().anchoredPosition;
        endPlayerHPSliderPosition = new Vector2(-startPlayerHPSliderPosition.x, startPlayerHPSliderPosition.y);
        endEnemyHPSliderPosition = new Vector2(-startEnemyHPSliderPosition.x, startEnemyHPSliderPosition.y);
        startProgressPanelPosition = progressPanel.GetComponent<RectTransform>().anchoredPosition;
        endProgressPanelPosition = new Vector2(startProgressPanelPosition.x, startProgressPanelPosition.y + 286f);

        // Display the current level in the main menu
        DisplayCurrentLevel();

        // Set up the character buttons
        if (LevelManager.instance.currentLevel.index != 0)
        {
            foreach (CharacterButton button in characterButtons)
            {
                var checkCharacter = SaveLoadManager.instance.ownedCharacters.Find(x => x.characterID == button.characterID);
                if (checkCharacter != null)//if found the same id in ownedCharacters list
                {
                    foreach (var name in checkCharacter.ownedSkins)
                    {
                        if (name == button.characterName)//if found the same name in ownedCharacters list
                        {
                            //Also used for buying
                            button.isOwned = true; // Set the button as owned
                            button.ownedText.gameObject.SetActive(true);
                            button.buyText.gameObject.SetActive(false);
                            button.lockImage.gameObject.SetActive(false);//Turn off the lock image
                            break;
                        }
                    }
                }
            }
        }

        //Load settings
        if (!SaveLoadManager.instance.isMusicOn)
        { 
            musicToggle.isOn = false;
        }

        if (!SaveLoadManager.instance.isSFXOn)
        {
            sfxToggle.isOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //ResetTimerSet();
    }

    #region MENU
    public void OnclickStartButton()
    {
        GameManager.instance.currentGameState = GameState.Playing;
        GameManager.instance.currentTurn = "None"; // Set the current turn to None

        StartCoroutine(ShowInGamePanel());

        //StartCoroutine(
        //CameraManager.instance.SetScreenPosComposition(1f, true, -0.25f));
        CameraManager.instance.StopAllCoroutines();
        CameraManager.instance.StartCoroutine(CameraManager.instance.SetHardLookAt(1f, 'Z', 0.7f));
        CameraManager.instance.StartCoroutine(CameraManager.instance.SetFollowOffset(0.5f, 'X', 0.7f));

        PlayerUltimate.instance.AddUltimateToUltiButton(PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().id);
        PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().SetHPSlider(true);
        StartCoroutine(ShowProgressPanel(0.3f));
        SetUsedProgress();
    }

    public void DisplayCurrentLevel()
    {
        if (LevelManager.instance.currentLevel != null)
        {
            if (LevelManager.instance.currentLevel.index == 0)
            {
                currentLevelDisplay.text = $"Tutorial";
            }
            else
            {
                currentLevelDisplay.text = $"Level {LevelManager.instance.currentLevel.index}";
            }

            if (LevelManager.instance.currentLevel.havingBoss)
            {
                skullImage.gameObject.SetActive(true);
            }
            else
            {
                skullImage.gameObject.SetActive(false);
            }
        }
        else
        {
            currentLevelDisplay.text = "Level Unknown";
        }
    }

    #endregion

    #region CURRENCY

    //Coin
    public void UpdateCoinText()
    {
        coinText.text = NumberFomatter.FormatIntToString(CurrencyManager.instance.coins, 2);
    }

    public IEnumerator CurrencyPanelZoomInAndZoomOut(string currency, float duration)
    {
        Transform targetObject;
        if (currency == "coin")
        {
            targetObject = coinPanel.transform;
        }
        else if (currency == "star")
        {
            targetObject = starPanel.transform;
        }
        else
        {
            targetObject = crystalPanel.transform;
        }

        float elaspedTime = 0f;
        Vector3 startScale = targetObject.localScale;
        Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f); // Zoom out scale
        while (elaspedTime < duration)
        {
            elaspedTime += Time.deltaTime;
            targetObject.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / duration);
            yield return null;
        }
        targetObject.localScale = targetScale; // Set the final scale

        //elaspedTime = 0f;
        //startScale = targetObject.localScale;
        //targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Zoom in scale
        //while (elaspedTime < 0.1f)
        //{
        //    elaspedTime += Time.deltaTime;
        //    targetObject.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.2f);
        //    yield return null;
        //}
        //targetObject.localScale = targetScale; // Set the final scale

        elaspedTime = 0f;
        startScale = targetObject.localScale;
        targetScale = new Vector3(1f, 1f, 1f); // Reset to original scale
        while (elaspedTime < duration)
        {
            elaspedTime += Time.deltaTime;
            targetObject.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / duration);
            yield return null;
        }
        targetObject.localScale = targetScale; // Set the final scale
    }

    public IEnumerator SpawnCurrencyPrefabAndMoveToPanel(Vector3 startTransform, string currency, int coinAmount, bool needChangeTransformFromWordToScreen)
    {
        string text = NumberFomatter.FormatIntToString(coinAmount, 2);
        Vector3 startPos;
        if (needChangeTransformFromWordToScreen)
        {
            startPos = Camera.main.WorldToScreenPoint(startTransform);
        }
        else
        {
            startPos = startTransform;
        }

        GameObject coinTextObject = null;
        if (currency == "coin")
        {
            //coinTextObject = Instantiate(coinTextPrefab, startPos, Quaternion.identity, transform);
            coinTextObject = PoolManager.Instance.GetObject("CoinText", startPos, Quaternion.identity, transform, coinTextPrefab);
        }
        else if (currency == "star")
        {
            //coinTextObject = Instantiate(starTextPrefab, startPos, Quaternion.identity, transform);
            coinTextObject = PoolManager.Instance.GetObject("StarText", startPos, Quaternion.identity, transform, starTextPrefab);
        }
        else
        {
            //coinTextObject = Instantiate(crystalTextPrefab, startPos, Quaternion.identity, transform);
            coinTextObject = PoolManager.Instance.GetObject("CrystalText", startPos, Quaternion.identity, transform, crystalTextPrefab);
        }
        AudioManager.instance.PlaySFX("DropCoinWhenEnemyNotDie");

        Text coinText = coinTextObject.GetComponent<Text>();
        RectTransform rectTransformCoinText = coinTextObject.GetComponent<RectTransform>();

        // Set the text and text size
        coinText.text = text;
        Vector2 textSize = rectTransformCoinText.sizeDelta;
        textSize.x = 60 + characterWidthCoinText * text.Length;
        rectTransformCoinText.sizeDelta = textSize;

        // Animation Text ZoomOut and Move Up
        float elaspedTime = 0f;
        rectTransformCoinText.localScale = Vector3.zero; // Start with a smaller scale
        Vector3 startScale = rectTransformCoinText.localScale;
        Vector3 targetScale = Vector3.one; // Zoom in scale
        while (elaspedTime < 0.15f) // Run this animation for 0.1s
        {
            elaspedTime += Time.deltaTime;
            rectTransformCoinText.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.1f);
            yield return null;
        }
        rectTransformCoinText.localScale = targetScale; // Set the final scale
        yield return new WaitForSeconds(0.25f); // Wait for a moment before moving

        elaspedTime = 0f;
        startScale = rectTransformCoinText.localScale;
        targetScale = Vector3.one * 0.6f; // Scale down 
        while (elaspedTime < 0.1f) // Run this animation for 0.2s
        {
            elaspedTime += Time.deltaTime;
            rectTransformCoinText.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.2f);
            yield return null;
        }
        rectTransformCoinText.localScale = targetScale; // Set the final scale

        elaspedTime = 0f;
        Vector3 targetPos = Vector3.zero;
        if(currency == "coin")
        {
            targetPos = coinPanel.transform.position;
        }
        else if (currency == "star")
        {
            targetPos = starPanel.transform.position;
        }
        else
        {
            targetPos = crystalPanel.transform.position;
        }
        while (elaspedTime < 0.2f) // Run this animation for 0.2s
        {
            elaspedTime += Time.deltaTime;
            rectTransformCoinText.position = Vector3.Lerp(startPos, targetPos, elaspedTime / 0.2f);
            yield return null;
        }
        rectTransformCoinText.position = targetPos; // Set the final position

        //Destroy(coinTextObject);
        if (currency == "coin")
        {
            //coinTextObject = Instantiate(coinTextPrefab, startPos, Quaternion.identity, transform);
            PoolManager.Instance.ReturnObject("CoinText", coinTextObject);
        }
        else if (currency == "star")
        {
            //coinTextObject = Instantiate(starTextPrefab, startPos, Quaternion.identity, transform);
            PoolManager.Instance.ReturnObject("StarText", coinTextObject);
        }
        else
        {
            //coinTextObject = Instantiate(crystalTextPrefab, startPos, Quaternion.identity, transform);
            PoolManager.Instance.ReturnObject("CrystalText", coinTextObject);
        }
    }

    //Crystal
    public void UpdateCrystalText()
    {
        crystalText.text = NumberFomatter.FormatIntToString(CurrencyManager.instance.crystals, 2);
    }

    //Star
    public void UpdateStarText()
    {
        starText.text = NumberFomatter.FormatIntToString(CurrencyManager.instance.stars, 2);
    }

    #endregion

    #region  CHARACTER AND SKIN

    public void SetBuyAndUpgradeCharacterButtonBasedOnCurrentChosenButton()
    {
        //Check if player already owned the character
        if (currentChosenButton.isOwned)
        {
            buyCharacterButton.gameObject.SetActive(false);
            if (SaveLoadManager.instance.currentLevelOfCurrentPlayer >= PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().bonusStatsLevel.Length)
            {
                upgradeCharacterButton.gameObject.SetActive(false); // Hide the upgrade button if the player has reached the max level
            }
            else
            {
                upgradeCharacterButton.gameObject.SetActive(true); // Show the upgrade button if the player can still upgrade
            }
        }
        else
        {
            buyCharacterButton.gameObject.SetActive(true);
            upgradeCharacterButton.gameObject.SetActive(false);
        }
    }

    public void SetUIInfoCurrentPlayer(int currentLevel)
    {
        var playerStat = PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>();
        SetBuyAndUpgradeCharacterButtonBasedOnCurrentChosenButton();

        //Change color for dam text
        string percentDamBonus;
        if (playerStat.bonusStatAtCurrentLevel.damagePercentBonus > 0)
        {
            percentDamBonus = $"+{NumberFomatter.FormatFloatToString(playerStat.bonusStatAtCurrentLevel.damagePercentBonus, 2)}";
            damTextInUI.color = Color.green;
        }
        else
        {
            percentDamBonus = $"{NumberFomatter.FormatFloatToString(playerStat.bonusStatAtCurrentLevel.damagePercentBonus, 2)}";
            if (playerStat.bonusStatAtCurrentLevel.damagePercentBonus < 0)
            {
                damTextInUI.color = Color.red;
            }
            else
            {
                damTextInUI.color = Color.white;
            }
        }

        //Change color for health text
        string percentHealthBonus;
        if (playerStat.bonusStatAtCurrentLevel.healthPercentBonus > 0)
        {
            percentHealthBonus = $"+{NumberFomatter.FormatFloatToString(playerStat.bonusStatAtCurrentLevel.healthPercentBonus, 2)}";
            healthTextInUI.color = Color.green;
        }
        else
        {
            percentHealthBonus = $"{NumberFomatter.FormatFloatToString(playerStat.bonusStatAtCurrentLevel.healthPercentBonus, 2)}";
            if (playerStat.bonusStatAtCurrentLevel.healthPercentBonus < 0)
            {
                healthTextInUI.color = Color.red;
            }
            else
            {
                healthTextInUI.color = Color.white;
            }
        }


        levelText.text = $"{NumberFomatter.FormatIntToString(currentLevel, 0)}/{playerStat.bonusStatsLevel.Length}";
        damTextInUI.text = $"{NumberFomatter.FormatFloatToString(playerStat.damage, 2)}" +
            $" ({percentDamBonus})";
        healthTextInUI.text = $"{NumberFomatter.FormatFloatToString(playerStat.maxHealth, 2)}" +
            $" ({percentHealthBonus})";

        if (playerStat.id == 0)
        {
            ultiDescriptionText.text = "Ulti: Increase your lifesteal for 1 turn. Lifesteal can heal based on damage dealt. " +
                "Throughout this turn, each attack restores a portion of lost health.";
            ultiStatText.text = $"Lifesteal: +0.1";
        }
        else if (playerStat.id == 1)
        {
            ultiDescriptionText.text = "Ulti: Increase your damage for 1 turn. This bonus damage based on your basic damage.";
            ultiStatText.text = $"Increased damage: +(0.15 x basic damage)";
        }
        else if (playerStat.id == 2)
        {
            ultiDescriptionText.text = "Ulti: A special item randomly appears, increasing damage to enemies for 1 turn. " +
                "Using it doesn’t end your turn.";
            ultiStatText.text = $"Damage taken: +10% for each connected fruit";
        }
    }

    public void OnChangeCharacterOrSkin(string characterName)
    {
        int id = -1;
        foreach (var button in characterButtons)
        {
            if (button.characterName == characterName)
            {
                id = button.characterID;
                break;
            }
        }

        if (id == -1)
        {
            Debug.LogError($"Character with name {characterName} not found in character buttons.");
            return;
        }

        //Check if the character exists in the owned characters list
        //Save the current player if the character is already owned
        int characterLevel = 1;
        var checkCharacter = SaveLoadManager.instance.ownedCharacters.Find(x => x.characterID == id);
        if (checkCharacter != null)
        {
            foreach (var name in checkCharacter.ownedSkins)
            {
                if (name == characterName)
                {
                    SaveLoadManager.instance.currentPlayerName = characterName;
                    SaveLoadManager.instance.currentLevelOfCurrentPlayer = checkCharacter.currentLevel;
                    characterLevel = checkCharacter.currentLevel;
                    break;
                }
            }
        }

        //Setup info player
        int previousCharacterID = PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().id;

        //Change character
        PlayerUltimate.instance.TurnOffAllPlayersTransform();
        PlayerUltimate.instance.GetPlayerTransform(characterName, characterLevel, 0.5f);

        //Set button
        SetCurrentChosenCharacterButton(id, characterName);

        //Setup info player
        if (previousCharacterID != id)
        {
            SetUIInfoCurrentPlayer(characterLevel);
        }
        SetBuyAndUpgradeCharacterButtonBasedOnCurrentChosenButton();
    }

    public void SetCurrentChosenCharacterButton(int id, string characterName)
    {
        foreach (CharacterButton button in characterButtons)
        {
            // Reset all buttons to not be chosen
            button.chosenImage.gameObject.SetActive(false);
            button.chosenButton.interactable = true; // Make all buttons interactable again
            if (button.characterID == id && button.characterName == characterName)
            {
                currentChosenButton = button;
                // Set the chosen button to be chosen
                button.chosenImage.gameObject.SetActive(true);
                button.chosenButton.interactable = false;
            }
        }
    }

    public void OnClickBuyCharacter(string currency)
    {
        var playerStat = PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>();

        //Check if it's a skin
        if (!playerStat.isNormalSkin)
        {
            //Check if the player already owned the character
            var checkCharacter = SaveLoadManager.instance.ownedCharacters.Find(x => x.characterID == playerStat.id);
            if (checkCharacter == null)
            {
                //Show panel to waring player, they need to buy character first
                titleBuyCharacterText.text = "You need to buy the character first before buying this skin!";
                titleBuyCharacterText.color = Color.red;
                return;
            }
        }

        if (currency == "coin")
        {
            //Check if the player not have enough coins
            if (CurrencyManager.instance.coins < playerStat.bonusStatsLevel[0].coinCost)
            {
                ShowWarningNotEnoughCostPanel("coin");
                return;
            }

            //Subtract coin
            CurrencyManager.instance.SubtractCoins(playerStat.bonusStatsLevel[0].coinCost);
        }
        else if (currency == "star")
        {
            if (CurrencyManager.instance.stars < playerStat.bonusStatsLevel[0].starCost)
            {
                ShowWarningNotEnoughCostPanel("star");
                return;
            }

            CurrencyManager.instance.SubtractStar(playerStat.bonusStatsLevel[0].starCost);
        }
        else
        {
            if (CurrencyManager.instance.crystals < playerStat.bonusStatsLevel[0].crystalCost)
            {
                ShowWarningNotEnoughCostPanel("crystal");
                return;
            }

            CurrencyManager.instance.SubtractCrystal(playerStat.bonusStatsLevel[0].crystalCost);
        }


        //Add character to owned characters
        PlayerUltimate.instance.SetUltimateAnimPlayer();
        if (playerStat.isNormalSkin)
        {
            SaveLoadManager.instance.ownedCharacters.Add(new OwnedCharacter(playerStat.id, playerStat.name));
            //Set character to current character
            SaveLoadManager.instance.currentPlayerName = playerStat.name;
            SaveLoadManager.instance.currentLevelOfCurrentPlayer = 1;
        }
        else
        {
            var ownedCharacter = SaveLoadManager.instance.ownedCharacters.Find(x => x.characterID == playerStat.id);
            if (ownedCharacter != null)
            {
                ownedCharacter.ownedSkins.Add(playerStat.name);
                //Set character to current character
                SaveLoadManager.instance.currentPlayerName = playerStat.name;
            }
            else
                Debug.LogError($"Character with ID {playerStat.id} not found in owned characters.");
        }

        //Setup UI
        currentChosenButton.isOwned = true; // Set the button as owned
        currentChosenButton.ownedText.gameObject.SetActive(true);
        currentChosenButton.buyText.gameObject.SetActive(false);
        currentChosenButton.lockImage.gameObject.SetActive(false);//Turn off the lock image

        buyCharacterButton.gameObject.SetActive(false);
        upgradeCharacterButton.gameObject.SetActive(true);

        HideBuyCharacterPanel();
    }

    public void OnClickUpgradeCharacter()
    {
        var playerStat = PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>();
        var nextBonusStat = playerStat.bonusStatsLevel[SaveLoadManager.instance.currentLevelOfCurrentPlayer];// Get the next bonus stat based on the current level

        //Check if the player not have enough stars
        if (CurrencyManager.instance.stars < nextBonusStat.starCost)
        {
            ShowWarningNotEnoughCostPanel("star");
            return;
        }
        //Check if the player not have enough coins
        if (CurrencyManager.instance.coins < nextBonusStat.coinCost)
        {
            ShowWarningNotEnoughCostPanel("coin");
            return;
        }

        //Subtract star and coin
        CurrencyManager.instance.SubtractStar(nextBonusStat.starCost);
        CurrencyManager.instance.SubtractCoins(nextBonusStat.coinCost);

        //Increase the level of the character
        if (SaveLoadManager.instance.currentLevelOfCurrentPlayer == playerStat.bonusStatsLevel.Length)
        {
            Debug.LogError("You have reached the max level of this character, cannot upgrade anymore!");
            return;
        }
        var ownedCharacter = SaveLoadManager.instance.ownedCharacters.Find(x => x.characterID == playerStat.id);
        if (ownedCharacter != null)
        {
            ownedCharacter.currentLevel++;
            SaveLoadManager.instance.currentLevelOfCurrentPlayer = ownedCharacter.currentLevel;
        }
        else
        {
            Debug.LogError($"Character with ID {playerStat.id} not found in owned characters.");
            return;
        }

        playerStat.bonusStatAtCurrentLevel = playerStat.bonusStatsLevel[SaveLoadManager.instance.currentLevelOfCurrentPlayer - 1]; // Set the bonus stats for the player at the current level
        playerStat.SetUpStatAndSlider();
        PlayerUltimate.instance.SetUpBaseStatForPlayer();

        //Setup UI
        SetUIInfoCurrentPlayer(SaveLoadManager.instance.currentLevelOfCurrentPlayer);
        HideUpgradeCharacterPanel();
    }

    #endregion

    #region UPGRADE
    public void OnClickUpgradeDamButton()
    {
        int cost = GameManager.instance.damCostToUpgrade;
        //Subtract coin
        if (CurrencyManager.instance.coins < cost)
        {
            return; // Not enough coins, exit the method
        }
        CurrencyManager.instance.SubtractCoins(cost);

        //Increase the damage level
        GameManager.instance.UpgradeDam();
        //Update Player ultimate
        PlayerUltimate.instance.SetUpBaseStatForPlayer();

        //Set UIButton
        SetUITextForUpgradeDamButton();
        UpdateColorTextForUpgradeCost();
    }

    public void OnClickUpgradeHealthButton()
    {
        int cost = GameManager.instance.healthCostToUpgrade;
        //Subtract coin
        if (CurrencyManager.instance.coins < cost)
        {
            return; // Not enough coins, exit the method
        }
        CurrencyManager.instance.SubtractCoins(cost);

        //Increase the health level
        GameManager.instance.UpgradeHealth();
        //Update Player ultimate
        PlayerUltimate.instance.SetUpBaseStatForPlayer();

        //Set UIButton
        SetUITextForUpgradeHealthButton();
        UpdateColorTextForUpgradeCost();
    }

    public void SetUITextForUpgradeDamButton()
    {
        damageLevelText.text = $"Damage\nLV.{NumberFomatter.FormatIntToString(GameManager.instance.currentDamageLevel, 0)}";
        basicDamageText.text = $"{NumberFomatter.FormatFloatToString(GameManager.instance.basicDamage, 2)}";
        costToUpgradeDamText.text = $"{NumberFomatter.FormatIntToString(GameManager.instance.damCostToUpgrade, 1)}";//Đưa hàm tính giá tiền vào đây
    }

    public void UpdateColorTextForUpgradeCost()
    {
        if (CurrencyManager.instance.coins < GameManager.instance.healthCostToUpgrade)
        {
            costToUpgradeHealthText.color = Color.red; // Change text color to red if not enough coins
        }
        else
        {
            costToUpgradeHealthText.color = Color.black; // Reset text color to white if enough coins
        }

        if (CurrencyManager.instance.coins < GameManager.instance.damCostToUpgrade)
        {
            costToUpgradeDamText.color = Color.red; // Change text color to red if not enough coins
        }
        else
        {
            costToUpgradeDamText.color = Color.black; // Reset text color to white if enough coins
        }
    }

    public void SetUITextForUpgradeHealthButton()
    {
        healthLevelText.text = $"Health\nLV.{NumberFomatter.FormatIntToString(GameManager.instance.currentHealthLevel, 0)}";
        basicHealthText.text = $"{NumberFomatter.FormatFloatToString(GameManager.instance.basicHealth, 2)}";
        costToUpgradeHealthText.text = $"{NumberFomatter.FormatFloatToString(GameManager.instance.healthCostToUpgrade, 2)}";//Đưa hàm tính giá tiền vào đây
    }
    #endregion

    #region IN GAME

    public void DisplayDamageText(Transform startTransform, Transform targetTransform, float dam)
    {
        string text = NumberFomatter.FormatFloatToString(dam, 2);
        Vector3 startPos = Camera.main.WorldToScreenPoint(startTransform.position);

        //GameObject damageTextObject = Instantiate(damageText, startPos, Quaternion.identity, transform);
        GameObject damageTextObject = PoolManager.Instance.GetObject("DamageText", startPos, Quaternion.identity, transform, damageText);

        Text damText = damageTextObject.GetComponent<Text>();
        RectTransform rectTransformDamText = damageTextObject.GetComponent<RectTransform>();

        // Set the text and text size
        damText.text = "-" + text;
        Vector2 textSize = rectTransformDamText.sizeDelta;
        textSize.x = 100 + characterWidthDamText * text.Length;
        rectTransformDamText.sizeDelta = textSize;

        //Animation Text ZoomOut and Move Up
        StartCoroutine(DisplayText(startPos, Camera.main.WorldToScreenPoint(targetTransform.position), rectTransformDamText));
    }

    IEnumerator DisplayText(Vector3 startPos, Vector3 targetPos, RectTransform text)
    {
        float elaspedTime = 0f;
        text.localScale = Vector3.zero; // Bắt đầu với tỉ lệ nhỏ hơn
        Vector3 startScale = text.localScale;
        Vector3 targetScale = Vector3.one;
        while (elaspedTime < 0.2f)//Chạy anim này trong vòng 0.1s
        {
            elaspedTime += Time.deltaTime;
            text.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.1f);
            yield return null;
        }
        text.localScale = targetScale; // Đặt tỉ lệ cuối cùng

        elaspedTime = 0f;
        while (elaspedTime < 0.2f)//Chạy anim này trong vòng 0.2s
        {
            elaspedTime += Time.deltaTime;
            text.position = Vector3.Lerp(startPos, targetPos, elaspedTime / 0.2f);
            yield return null;
        }
        text.position = targetPos; // Đặt vị trí cuối cùng

        yield return new WaitForSeconds(0.25f); // Đợi một chút trước khi xóa text

        //Destroy(text.gameObject); // Xóa text sau khi hoàn thành
        PoolManager.Instance.ReturnObject("DamageText", text.gameObject);
    }

    public IEnumerator IncreaseManaSliderValue(float duration, float targetValue)
    {
        if (targetValue >= PlayerUltimate.instance.maxMana)
        {
            yield return null;
        }

        float elapsedTime = 0f;
        float startValue = manaSlider.value;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            manaSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }
        manaSlider.value = targetValue; // Set the final value
    }

    public IEnumerator HideManaSlider(float duration)
    {
        RectTransform rectTransform = manaSlider.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, startPosYManaSlider - 160f);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos; // Set the final position
        manaSlider.gameObject.SetActive(false); // Hide the slider after the animation
    }

    public IEnumerator AppearManaSlider(float duration)
    {
        manaSlider.gameObject.SetActive(true); // Ensure the slider is active
        RectTransform rectTransform = manaSlider.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, startPosYManaSlider);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos; // Set the final position
    }

    public IEnumerator ShowUltimateButtonAnim(float duration)
    {
        ultimateButton.transform.localScale = Vector3.zero; // Start with the button hidden
        ultimateButtonAndEffectObject.SetActive(true); // Ensure the ultimate button and effect object is active
        ultimateButton.gameObject.SetActive(true); // Ensure the button is active
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            ultimateButton.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsedTime / duration);
            yield return null;
        }
        ultimateButton.transform.localScale = Vector3.one; // Ensure the final scale is set to 1
    }

    public IEnumerator ChangeManaSliderAndUltimateButton()
    {
        yield return StartCoroutine(HideManaSlider(0.2f)); // Show the mana slider
        StartCoroutine(ShowUltimateButtonAnim(0.1f)); // Show the ultimate button
    }

    public IEnumerator AppearHPSlider(bool isPlayer, float duration)
    {
        float elapsedTime = 0f;
        if (isPlayer)
        {
            playerHPSlider.GetComponent<RectTransform>().anchoredPosition = endPlayerHPSliderPosition; // Start from the end position
            playerHPSlider.gameObject.SetActive(true); // Ensure the player HP slider is active
            while (elapsedTime < duration)
            {
                playerHPSlider.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(endPlayerHPSliderPosition, startPlayerHPSliderPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            playerHPSlider.GetComponent<RectTransform>().anchoredPosition = startPlayerHPSliderPosition; // Set the final position
        }
        else
        {
            enemyHPSlider.GetComponent<RectTransform>().anchoredPosition = endEnemyHPSliderPosition; // Start from the end position
            enemyHPSlider.gameObject.SetActive(true); // Ensure the enemy HP slider is active
            while (elapsedTime < duration)
            {
                enemyHPSlider.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(endEnemyHPSliderPosition, startEnemyHPSliderPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            enemyHPSlider.GetComponent<RectTransform>().anchoredPosition = startEnemyHPSliderPosition; // Set the final position
        }
    }

    public IEnumerator HideHPSlider(bool isPlayer, float duration)
    {
        float elapsedTime = 0f;
        if (isPlayer)
        {
            while (elapsedTime < duration)
            {
                playerHPSlider.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPlayerHPSliderPosition, endPlayerHPSliderPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            playerHPSlider.GetComponent<RectTransform>().anchoredPosition = endPlayerHPSliderPosition; // Set the final position
            playerHPSlider.gameObject.SetActive(false); // Hide the player HP slider after the animation
        }
        else
        {
            while (elapsedTime < duration)
            {
                enemyHPSlider.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startEnemyHPSliderPosition, endEnemyHPSliderPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            enemyHPSlider.GetComponent<RectTransform>().anchoredPosition = endEnemyHPSliderPosition; // Set the final position
            enemyHPSlider.gameObject.SetActive(false); // Hide the enemy HP slider after the animation
        }

    }

    public void SetProgressAtStart()
    {
        usedProgress.Clear();

        int amountOfProgress = LevelManager.instance.currentLevel.enemiesAtLevel.Count;
        if (!LevelManager.instance.currentLevel.havingBoss)
        {
            for (int i = 0; i < gameProgress.Length; i++)
            {
                if (i < amountOfProgress)
                {
                    gameProgress[i].gameObject.SetActive(true); // Ensure the progress indicators are active
                    usedProgress.Add(gameProgress[i]);
                }
                else
                {
                    gameProgress[i].gameObject.SetActive(false); // Hide the progress indicators that are not needed
                }
            }
        }
        else
        {
            for (int i = 0; i < gameProgress.Length - 1; i++)
            {
                if (i < amountOfProgress - 1)
                {
                    gameProgress[i].gameObject.SetActive(true); // Ensure the progress indicators are active
                    usedProgress.Add(gameProgress[i]);
                }
                else
                {
                    gameProgress[i].gameObject.SetActive(false); // Hide the progress indicators that are not needed
                }
            }
            gameProgress[gameProgress.Length - 1].gameObject.SetActive(true); // Ensure the boss progress indicator is active
            usedProgress.Add(gameProgress[gameProgress.Length - 1]); // Add the boss progress indicator to the used list
        }
    }

    public void SetUsedProgress()
    {
        Color defaultColor = Color.black;
        defaultColor.a = 0.6f;
        foreach (var progress in usedProgress)
        {
            progress.GetComponent<Image>().color = defaultColor;
        }
    }

    public void SetCurrentProgress(bool isDone)
    {
        if (isDone)
        {
            usedProgress[GameManager.instance.currentEnemyIndex].GetComponent<Image>().color = Color.green; // Set the color to green if the progress is done
        }
        else
        {
            Color doingColor = Color.red;
            doingColor.a = 1f; // Set the alpha to 0.6f for the doing color
            usedProgress[GameManager.instance.currentEnemyIndex].GetComponent<Image>().color = doingColor;
        }
    }

    public IEnumerator ShowProgressPanel(float duration)
    {
        float elapsedTime = 0f;
        progressPanel.GetComponent<RectTransform>().anchoredPosition = endProgressPanelPosition; // Start from the end position
        progressPanel.SetActive(true);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            progressPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(endProgressPanelPosition, startProgressPanelPosition, elapsedTime / duration);
            yield return null;
        }
        progressPanel.GetComponent<RectTransform>().anchoredPosition = startProgressPanelPosition; // Set the final position
    }

    public IEnumerator HideProgressPanel(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            progressPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startProgressPanelPosition, endProgressPanelPosition, elapsedTime / duration);
            yield return null;
        }
        progressPanel.GetComponent<RectTransform>().anchoredPosition = endProgressPanelPosition; // Set the final position
        progressPanel.SetActive(false); // Hide the progress panel after the animation
    }

    public void HideAllHUD()
    {
        StartCoroutine(HideHPSlider(true, 0.3f));
        StartCoroutine(HideHPSlider(false, 0.3f));
        StartCoroutine(HideProgressPanel(0.3f)); // Hide the progress panel
    }

    public void HideAllHUDWithoutAnim()
    {
        progressPanel.SetActive(false);
        playerHPSlider.gameObject.SetActive(false);
        enemyHPSlider.gameObject.SetActive(false);
    }

    #endregion

    #region SETTING
    public void OnCLickReturnMenuButton()
    {
        OnCLickCloseSettingPanel();
        GameManager.instance.StartCoroutine(GameManager.instance.LoadNewLevel());
    }

    public void OnClickSettingButton()
    {
        settingPanel.SetActive(true);
    }

    public void OnClickMusicToggle()
    {

        AudioManager.instance.musicSource.mute = !musicToggle.isOn;
        SaveLoadManager.instance.isMusicOn = !AudioManager.instance.musicSource.mute;
        PlayerPrefs.SetInt("IsMusicOn", !AudioManager.instance.musicSource.mute ? 1 : 0);
    }

    public void OnClickSoundToggle()
    {
        AudioManager.instance.sfxSource.mute = !sfxToggle.isOn;
        SaveLoadManager.instance.isSFXOn = !AudioManager.instance.sfxSource.mute;
        PlayerPrefs.SetInt("IsSFXOn", !AudioManager.instance.sfxSource.mute ? 1 : 0);
    }

    public void OnCLickCloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    #endregion

    #region SHOW PANEL

    public void HideCurrentLevelText()
    {
        currentLevelDisplay.gameObject.SetActive(false); // Hide the current level text
    }

    public void ShowCurrentLevelText()
    {
        currentLevelDisplay.gameObject.SetActive(true); // Show the current level text
    }

    public IEnumerator ShowInGamePanel()
    {
        gameBoard.SetActive(false);
        inGamePanel.SetActive(true);
        yield return StartCoroutine(SlidePanel( mainMenuPanel.GetComponent<RectTransform>(), originaloffsetMinMenuPanel, 
            originaloffsetMaxMenuPanel, originaloffsetMinMenuPanel - new Vector2(0, 1920f), originaloffsetMaxMenuPanel - new Vector2(0, 1920f), 0.5f));
        mainMenuPanel.SetActive(false);
        ultimateButtonAndEffectObject.SetActive(false);
        ultimateButton.gameObject.SetActive(false);
        manaSlider.gameObject.SetActive(true);
        StartCoroutine(ShowPanelWithZoomInAnim(gameBoard, 0.5f, Vector3.zero));
        //foreach (Image tabsButtons in tabsManager.tabButtons)
        //{
        //    tabsButtons.gameObject.SetActive(false); // Hide all tab buttons
        //}
        if (GameBoard.Instance.GetTutorial())
        {
            GameBoard.Instance.ResetGuideStep();
        }
    }

    public void ShowGameOverPanel(bool isPlayerWin)
    {
        StartCoroutine(ShowOverPanel());
        mainMenuPanel.SetActive(false);
        //inGamePanel.SetActive(false);
    }

    public void ShowMainMenuPanel()
    {
        // Reset the main menu panel position to its original state
        mainMenuPanel.GetComponent<RectTransform>().offsetMin = originaloffsetMinMenuPanel; // Reset the offsetMin
        mainMenuPanel.GetComponent<RectTransform>().offsetMax = originaloffsetMaxMenuPanel; // Reset the offsetMax
        mainMenuPanel.SetActive(true);
        //StartCoroutine(SlidePanel(mainMenuPanel.GetComponent<RectTransform>(), mainMenuPanel.GetComponent<RectTransform>().offsetMin,
        //    mainMenuPanel.GetComponent<RectTransform>().offsetMax, originaloffsetMinMenuPanel, originaloffsetMaxMenuPanel, 1f));
        inGamePanel.SetActive(false);
        HideOverPanel();
        SetProgressAtStart();

        //foreach (Image tabsButtons in tabsManager.tabButtons)
        //{
        //    tabsButtons.gameObject.SetActive(true); // Hide all tab buttons
        //}
    }

    public IEnumerator ShowPanelWithZoomInAnim(GameObject panel, float duration, Vector3 startScale)
    {
        float elapsedTime = 0f;
        panel.transform.localScale = startScale; // Start with the panel hidden
        //Vector3 startScale = panel.transform.localScale;
        Vector3 targetScale = Vector3.one; // Zoom in scale
        panel.SetActive(true); // Ensure the panel is active before starting the animation
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            panel.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            yield return null;
        }
        panel.transform.localScale = targetScale; // Set the final scale
    }

    public IEnumerator HidePanelWithZoomOutAnim(GameObject panel, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startScale = panel.transform.localScale;
        Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f); // Zoom out scale
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            panel.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            yield return null;
        }
        panel.transform.localScale = targetScale; // Set the final scale
        panel.SetActive(false); // Hide the panel after the animation
    }

    public IEnumerator SlidePanel(RectTransform panel, Vector2 StartMin, Vector2 startMax, Vector2 targetMin, Vector2 targetMax , float duration)
    { 
        float elapsedTime = 0f;
        panel.offsetMin = StartMin; // Start from the initial position
        panel.offsetMax = startMax; // Start from the initial position
        AudioManager.instance.PlaySFX("UIMove");
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            panel.offsetMin = Vector2.Lerp(StartMin, targetMin, elapsedTime / duration);
            panel.offsetMax = Vector2.Lerp(startMax, targetMax, elapsedTime / duration);
            yield return null;
        }
        panel.offsetMin = targetMin; // Set the final position
        panel.offsetMax = targetMax; // Set the final position
    }

    public void HideBuyAndUpgradeCharacterPanel()
    {
        buyAndUpgradeCharacterPanel.SetActive(false); // Hide the buy and upgrade character panel
    }

    public void ShowBuyCharacterPanel()
    {
        var playerStat = PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>();

        if (playerStat.isNormalSkin)
        {
            titleBuyCharacterText.text = "You want to buy this character?";
            titleBuyCharacterText.color = Color.white;
        }
        else
        {
            titleBuyCharacterText.text = "You want to buy this skin?";
            titleBuyCharacterText.color = Color.white;
        }

        // Set the text for buying character
        if (playerStat.bonusStatsLevel[0].coinCost <= 0)
        {
            useCoinToBuyCharacterButton.gameObject.SetActive(false); // Hide the button if cost is 0
        }
        else
        {
            useCoinToBuyCharacterButton.gameObject.SetActive(true); // Show the button if cost is greater than 0
            useCoinToBuyCharacterText.text = $"{NumberFomatter.FormatIntToString(playerStat.bonusStatsLevel[0].coinCost, 2)}";
            if (CurrencyManager.instance.coins < playerStat.bonusStatsLevel[0].coinCost)
            {
                useCoinToBuyCharacterText.color = Color.red; // Change text color to red if not enough coins
            }
            else
            {
                useCoinToBuyCharacterText.color = Color.white; // Reset text color to white if enough coins
            }
        }

        if (playerStat.bonusStatsLevel[0].crystalCost <= 0)
        {
            useCrystalToBuyCharacterButton.gameObject.SetActive(false); // Hide the button if cost is 0
        }
        else
        {
            useCrystalToBuyCharacterButton.gameObject.SetActive(true); // Show the button if cost is greater than 0
            useCrystalToBuyCharacterText.text = $"{NumberFomatter.FormatIntToString(playerStat.bonusStatsLevel[0].crystalCost, 2)}";
            if (CurrencyManager.instance.crystals < playerStat.bonusStatsLevel[0].crystalCost)
            {
                useCrystalToBuyCharacterText.color = Color.red; // Change text color to red if not enough coins
            }
            else
            {
                useCrystalToBuyCharacterText.color = Color.white; // Reset text color to white if enough coins
            }
        }

        if (playerStat.bonusStatsLevel[0].starCost <= 0)
        {
            useStarToBuyCharacterButton.gameObject.SetActive(false); // Hide the button if cost is 0
        }
        else
        {
            useStarToBuyCharacterButton.gameObject.SetActive(true); // Show the button if cost is greater than 0
            useStarToBuyCharacterText.text = $"{NumberFomatter.FormatIntToString(playerStat.bonusStatsLevel[0].starCost, 2)}";
            if (CurrencyManager.instance.stars < playerStat.bonusStatsLevel[0].starCost)
            {
                useStarToBuyCharacterText.color = Color.red; // Change text color to red if not enough coins
            }
            else
            {
                useStarToBuyCharacterText.color = Color.white; // Reset text color to white if enough coins
            }
        }

        buyAndUpgradeCharacterPanel.SetActive(true); // Show the buy and upgrade character panel
        StartCoroutine(ShowPanelWithZoomInAnim(BuyCharacterPanel, 0.15f, new Vector3(0.6f, 0.6f, 0.6f)));
    }

    public void ShowUpgradeCharacterPanel()
    {
        var playerStat = PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>();
        var nextBonusStat = playerStat.bonusStatsLevel[SaveLoadManager.instance.currentLevelOfCurrentPlayer];// Get the next bonus stat based on the current level

        //Set title for upgrade character panel
        titleUpgradeCharacterText.text = $"Upgrade {playerStat.name}";

        //Change color for dam text
        string currentPercentDamBonus;
        string newPercentDamBonus;
        if (playerStat.bonusStatAtCurrentLevel.damagePercentBonus > 0)
        {
            currentPercentDamBonus = $"+{NumberFomatter.FormatFloatToString(playerStat.bonusStatAtCurrentLevel.damagePercentBonus, 2)}";
            newPercentDamBonus = $"+{NumberFomatter.FormatFloatToString(nextBonusStat.damagePercentBonus, 2)}";
        }
        else
        {
            currentPercentDamBonus = $"{NumberFomatter.FormatFloatToString(playerStat.bonusStatAtCurrentLevel.damagePercentBonus, 2)}";
            newPercentDamBonus = $"{NumberFomatter.FormatFloatToString(nextBonusStat.damagePercentBonus, 2)}";
        }

        //Change color for health text
        string currentPercentHealthBonus;
        string newPercentHealthBonus;
        if (playerStat.bonusStatAtCurrentLevel.healthPercentBonus > 0)
        {
            currentPercentHealthBonus = $"+{NumberFomatter.FormatFloatToString(playerStat.bonusStatAtCurrentLevel.healthPercentBonus, 2)}";
            newPercentHealthBonus = $"+{NumberFomatter.FormatFloatToString(nextBonusStat.healthPercentBonus, 2)}";
        }
        else
        {
            currentPercentHealthBonus = $"{NumberFomatter.FormatFloatToString(playerStat.bonusStatAtCurrentLevel.healthPercentBonus, 2)}";
            newPercentHealthBonus = $"{NumberFomatter.FormatFloatToString(nextBonusStat.healthPercentBonus, 2)}";
        }

        //Set upgrade info
        upgradeDamStatText.text = $"Damage: {currentPercentDamBonus}";
        upgradeDamStatAtNewLevelText.text = $"{newPercentDamBonus}";
        upgradeHealthStatText.text = $"Health: {currentPercentHealthBonus}";
        upgradeHealthStatAtNewLevelText.text = $"{newPercentHealthBonus}";

        //Set cost and button cost
        starToUpgradeText.text = $"{NumberFomatter.FormatIntToString(nextBonusStat.starCost, 2)}";
        if (CurrencyManager.instance.stars < nextBonusStat.starCost)
        {
            starToUpgradeText.color = Color.red; // Change text color to red if not enough coins
        }
        else
        {
            starToUpgradeText.color = Color.white; // Reset text color to white if enough coins
        }

        coinToUpgradeText.text = $"{NumberFomatter.FormatIntToString(nextBonusStat.coinCost, 2)}";
        if (CurrencyManager.instance.coins < nextBonusStat.coinCost)
        {
            coinToUpgradeText.color = Color.red; // Change text color to red if not enough coins
        }
        else
        {
            coinToUpgradeText.color = Color.white; // Reset text color to white if enough coins
        }

        buyAndUpgradeCharacterPanel.SetActive(true); // Show the buy and upgrade character panel
        StartCoroutine(ShowPanelWithZoomInAnim(UpgradeCharacterPanel, 0.15f, new Vector3(0.6f, 0.6f, 0.6f)));
    }

    public void ShowWarningNotEnoughCostPanel(string currency)
    {
        if (currency == "coin")
        {
            warningNotEnoughCostText.text = "You don't have enough coins to buy this character!";
        }
        else if (currency == "star")
        {
            warningNotEnoughCostText.text = "You don't have enough stars to buy this character!";
        }
        else
        {
            warningNotEnoughCostText.text = "You don't have enough crystals to buy this character!";
        }
        quitBuyCharacterPanelButton.gameObject.SetActive(false); // Hide the quit button in the buy character panel
        quitUpgradeCharacterPanelButton.gameObject.SetActive(false); // Hide the quit button in the upgrade character panel
        StartCoroutine(ShowPanelWithZoomInAnim(WarningNotEnoughCostPanel, 0.15f, new Vector3(0.6f, 0.6f, 0.6f)));
    }

    public void HideBuyCharacterPanel()
    {
        StartCoroutine(HidePanelWithZoomOutAnim(BuyCharacterPanel, 0.2f));
        Invoke(nameof(HideBuyAndUpgradeCharacterPanel), 0.2f); // Hide the buy and upgrade character panel after the animation
    }

    public void HideUpgradeCharacterPanel()
    {
        StartCoroutine(HidePanelWithZoomOutAnim(UpgradeCharacterPanel, 0.2f));
        Invoke(nameof(HideBuyAndUpgradeCharacterPanel), 0.2f); // Hide the buy and upgrade character panel after the animation
    }

    public void HideWarningNotEnoughCostPanel()
    {
        quitBuyCharacterPanelButton.gameObject.SetActive(true); // Show the quit button in the buy character panel
        quitUpgradeCharacterPanelButton.gameObject.SetActive(true); // Show the quit button in the upgrade character panel
        StartCoroutine(HidePanelWithZoomOutAnim(WarningNotEnoughCostPanel, 0.2f));
    }

    #endregion

    #region GAME OVER

    public IEnumerator ShowOverPanel()
    {
        if (GameManager.instance.currentTurn == "Win")
        { 
            gameOverPanelImage.sprite = winPanelSprite;
            resultImage.sprite = winResultSprite;
            returnMenuButton.GetComponent<Image>().sprite = winButtonSprite;
            rewardCoinText.text = "0";
            rewardPanel.SetActive(true);
        }
        else
        {
            gameOverPanelImage.sprite = losePanelSprite;
            resultImage.sprite = loseResultSprite;
            returnMenuButton.GetComponent<Image>().sprite = loseButtonSprite;
            rewardPanel.SetActive(false);
        }

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(HidePanelWithZoomOutAnim(gameBoard, 0.5f));
        parentObject.SetActive(true);

        yield return StartCoroutine(ShowPanelWithZoomInAnim(gameOverPanel, 0.3f, new Vector3(0.6f, 0.6f, 0.6f)));

        // Count coins
        if (GameManager.instance.currentTurn == "Win")
        { 
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(CountCoin(LevelManager.instance.currentLevel.rewardCoin, 1f) );
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(CurrencyManager.instance.AddCoins(rewardCoinText.transform.position, LevelManager.instance.currentLevel.rewardCoin, false, 0f));

            //Save new level
            LevelManager.instance.SetNextLevel();
        }

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(ShowPanelWithZoomInAnim(returnMenuButton.gameObject, 0.15f, new Vector3(0.6f, 0.6f, 0.6f)));
    }

    public IEnumerator CountCoin(int targetCoin, float duration)
    {
        int startScore = 0;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int currentCoin = Mathf.RoundToInt(Mathf.Lerp(startScore, targetCoin, t));
            rewardCoinText.text = currentCoin.ToString();
            yield return null;
        }

        // đảm bảo kết quả cuối cùng chính xác
        rewardCoinText.text = targetCoin.ToString();
    }

    public void HideOverPanel()
    { 
        returnMenuButton.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
        parentObject.SetActive(false);
    }

    public void OnClickOverGameButton()
    {
        GameManager.instance.StartCoroutine(GameManager.instance.LoadNewLevel());
    }
    #endregion

    #region MISSIONS
    public void SetUpMissionsDescription()
    {
        for (int i = 0; i < missionsDescriptionTexts.Length; i++)
        {
            //if (MissionsManager._instance.missions[i].isActive && !MissionsManager._instance.missions[i].isCompleted)
            //{
                // Set the description based on the mission type
                if (MissionsManager._instance.missions[i].missionType == MissionType.KillEnemy)
                {
                    missionsDescriptionTexts[i].text = MissionsManager._instance.missions[i].description;
                    Debug.Log("Enemy Killed: " + MissionsManager._instance.missions[i].goal.currentAmount + "/" + MissionsManager._instance.missions[i].goal.targetAmount);
                }
                else if (MissionsManager._instance.missions[i].missionType == MissionType.FruitMatching)
                {
                    missionsDescriptionTexts[i].text = MissionsManager._instance.missions[i].description;
                    Debug.Log("Fruit Matched: " + MissionsManager._instance.missions[i].goal.currentAmount + "/" + MissionsManager._instance.missions[i].goal.targetAmount);
                }
                else if (MissionsManager._instance.missions[i].missionType == MissionType.UpgradeDamageStats)
                {
                    missionsDescriptionTexts[i].text = MissionsManager._instance.missions[i].description;
                    Debug.Log("Damage Upgraded: " + MissionsManager._instance.missions[i].goal.currentAmount + "/" + MissionsManager._instance.missions[i].goal.targetAmount);
                }
                else if (MissionsManager._instance.missions[i].missionType == MissionType.UpgradeHealthStats)
                {
                    missionsDescriptionTexts[i].text = MissionsManager._instance.missions[i].description;
                    Debug.Log("Health Upgraded: " + MissionsManager._instance.missions[i].goal.currentAmount + "/" + MissionsManager._instance.missions[i].goal.targetAmount);
                }
                else if (MissionsManager._instance.missions[i].missionType == MissionType.ReachLevel)
                {
                    missionsDescriptionTexts[i].text = MissionsManager._instance.missions[i].description;
                    Debug.Log("Level Reached: " + MissionsManager._instance.missions[i].goal.currentAmount + "/" + MissionsManager._instance.missions[i].goal.targetAmount);
                }
                else if (MissionsManager._instance.missions[i].missionType == MissionType.UsePowerUp)
                {
                    missionsDescriptionTexts[i].text = MissionsManager._instance.missions[i].description;
                    Debug.Log("Power Up Used: " + MissionsManager._instance.missions[i].goal.currentAmount + "/" + MissionsManager._instance.missions[i].goal.targetAmount);
                }
                //rewardAmount[i].text = "Reward: " + missions[i].reward.ToString();
            //}
            //else
            //{
            //    missionsDescriptionTexts[i].text = "No active mission";
            //}
            IsMissionCompletedChecking();
            UpdateMissionsProgress();
        }
    }

    public void UpdateMissionsProgress()
    {
        for (int i = 0; i < missionsProgressSliders.Length; i++)
        {
            if (MissionsManager._instance.missions[i].isActive && !MissionsManager._instance.missions[i].isCompleted)
            {
                missionsProgressSliders[i].maxValue = MissionsManager._instance.missions[i].goal.targetAmount;
                missionsProgressSliders[i].value = MissionsManager._instance.missions[i].goal.currentAmount;
                missionsProgressTexts[i].text = $"{MissionsManager._instance.missions[i].goal.currentAmount}/{MissionsManager._instance.missions[i].goal.targetAmount}";
            }
            else if (MissionsManager._instance.missions[i].isCompleted)
            {
                missionsProgressSliders[i].maxValue = MissionsManager._instance.missions[i].goal.targetAmount;
                missionsProgressSliders[i].value = MissionsManager._instance.missions[i].goal.targetAmount;
                missionsProgressTexts[i].text = $"{MissionsManager._instance.missions[i].goal.targetAmount}/{MissionsManager._instance.missions[i].goal.targetAmount}";
            }
            else
            {
                missionsProgressSliders[i].maxValue = 1;
                missionsProgressSliders[i].value = 0;
                missionsProgressTexts[i].text = "0/0";
            }
        }

        chestSlider.maxValue = MissionsManager._instance.missions.Length;
        chestSlider.value = MissionsManager._instance.missionCompletedCount;
        chestProgressText.text = $"{MissionsManager._instance.missionCompletedCount}/{MissionsManager._instance.missions.Length}";
    }

    public void IsMissionCompletedChecking()
    {       
        for (int i = 0; i < missionsCompletedClaimButtons.Length; i++)
        {
            if (MissionsManager._instance.missions[i].isCompleted && !MissionsManager._instance.missions[i].isClaimed)
            {
                missionsCompletedClaimButtons[i].SetActive(true); // Enable the claim button if the mission is completed and not yet claimed
                missionCompletedImage[i].gameObject.SetActive(false); // Show the completed image
            }
            else if (MissionsManager._instance.missions[i].isCompleted && MissionsManager._instance.missions[i].isClaimed)
            {
                missionsCompletedClaimButtons[i].SetActive(false); // Disable the claim button if the mission is already claimed
                missionCompletedImage[i].gameObject.SetActive(true); // Show the completed image
            }
            else
            {
                missionsCompletedClaimButtons[i].SetActive(false); // Disable the claim button otherwise
                missionCompletedImage[i].gameObject.SetActive(false); // Show the completed image
            }
        }
        
    }

    public void MissionsRewardClaiming()
    {
        MissionsManager._instance.RewardClaiming();
        IsMissionCompletedChecking();
        UpdateMissionsProgress();
    }

    public IEnumerator ResetTimerSet()
    {

        while (true)
        {
            if (Timer.Instance.GetRemainingTime() == 0)
            {
                Timer.Instance.ResetMissionTime();
                MissionsManager._instance.ResetMission();
                SetUpMissionsDescription();
                Debug.Log("Missions have been reset after timer reached zero.");
            }
            resetMissionTimer.text = Timer.Instance.GetFormattedTime();
            yield return null;
        }
    }

    public void ChestClaiming()
    {
        if (MissionsManager._instance.ChestOpening() == true)
        {
            chestManager.OpenChest();
        }
    }
    #endregion

    #region CHEST BOX

    public void ShowChestBoxPanel(List<Item> itemRewards, int crystals, int stars, int coins)
    {
        pressCount = 0;
        itemRewardsInChestBox = itemRewards;
        crystalsInChestBox = crystals;
        starsInChestBox = stars;
        coinsInChestBox = coins;

        openChestBoxPanel.SetActive(true); // Show the chest box panel
        StartCoroutine(ShowRewardInChestBox());
    }

    public void OnClickOpenChestBoxPanel()
    {
        if(isShowRewardRunning) return; // If the reward display is already running, do not allow another click

        pressCount++;
        StartCoroutine(ShowRewardInChestBox()); // Start showing the rewards in the chest box panel
    }

    public IEnumerator ShowRewardInChestBox()
    {
        isShowRewardRunning = true; // Set the flag to indicate that the reward display is running

        if (pressCount == 0)
        {
            //anim open chest
            AudioManager.instance.PlaySFX("ChestAppear");
            yield return StartCoroutine(ShowPanelWithZoomInAnim(chestBoxImage.gameObject, 0.15f, Vector3.zero));
            yield return new WaitForSeconds(0.35f);
            yield return StartCoroutine(OpenChestBoxAnim(1f)); // Start the chest box opening animation
            yield return new WaitForSeconds(0.5f); // Wait for the animation to finish before showing the rewards
        }

        // Show the rewards in the chest box panel
        GameObject itemRewardPrefab = null;
        if (itemRewardsInChestBox != null && itemRewardsInChestBox.Count > 0 && pressCount < itemRewardsInChestBox.Count)
        {
            itemRewardPrefab = Instantiate(itemBonusPrefab, chestBoxImage.transform.position, Quaternion.identity, transform);
            itemRewardPrefab.GetComponent<UIRewardInChestBox>().SetUI(itemRewardsInChestBox[pressCount].itemLevel, itemRewardsInChestBox[pressCount].itemType);
        }
        else if (crystalsInChestBox > 0 && pressCount == itemRewardsInChestBox.Count)
        {
            itemRewardPrefab = Instantiate(crystalBonusPrefab, chestBoxImage.transform.position, Quaternion.identity, transform);
            itemRewardPrefab.transform.GetChild(1).GetComponent<Text>().text = $"{NumberFomatter.FormatIntToString(crystalsInChestBox, 2)}"; // Set the text for crystal amount
            isClaimCrystal = true; // Set the flag to indicate that crystals are being claimed
        }
        else if (starsInChestBox > 0 && pressCount == itemRewardsInChestBox.Count + 1)
        {
            itemRewardPrefab = Instantiate(starBonusPrefab, chestBoxImage.transform.position, Quaternion.identity, transform);
            itemRewardPrefab.transform.GetChild(1).GetComponent<Text>().text = $"{NumberFomatter.FormatIntToString(starsInChestBox, 2)}"; // Set the text for star amount
            isClaimStar = true;
        }
        else if (coinsInChestBox > 0 && pressCount == itemRewardsInChestBox.Count + 2)
        {
            itemRewardPrefab = Instantiate(coinBonusPrefab, chestBoxImage.transform.position, Quaternion.identity, transform);
            itemRewardPrefab.transform.GetChild(1).GetComponent<Text>().text = $"{NumberFomatter.FormatIntToString(coinsInChestBox, 2)}"; // Set the text for coin amount
            isClaimCoin = true;
        }
        else
        {
            if (!claimRewardButton.gameObject.activeSelf && pressCount >= itemRewardsInChestBox.Count + 2)
            {
                StartCoroutine(ShowPanelWithZoomInAnim(claimRewardButton.gameObject, 0.15f, new Vector3(0.6f, 0.6f, 0.6f)));
            }

            isShowRewardRunning = false; // Reset the flag to indicate that the reward display is done
            yield break; // Exit the coroutine if there are no more rewards to show
        }

        yield return StartCoroutine(ShowOneReward(itemRewardPrefab, 0.25f));
        yield return new WaitForSeconds(0.75f); // Wait for a short duration before showing the next reward

        if (pressCount < itemRewardsInChestBox.Count)
        {
            StartCoroutine(HideOneReward(itemRewardPrefab, 0.2f, new Vector3(0.6f, 0.6f, 0.6f), itemParent.transform)); // Hide the item reward prefab with zoom out animation
        }
        else
        {
            StartCoroutine(HideOneReward(itemRewardPrefab, 0.2f, new Vector3(0.6f, 0.6f, 0.6f), currencyParent.transform)); // Hide the currency reward prefab with zoom out animation

            if (pressCount == itemRewardsInChestBox.Count + 2)
            {
                StartCoroutine(ShowPanelWithZoomInAnim(claimRewardButton.gameObject, 0.15f, new Vector3(0.6f, 0.6f, 0.6f)));
            }
        }

        isShowRewardRunning = false; // Reset the flag to indicate that the reward display is done
    }

    public IEnumerator ShowOneReward(GameObject rewardObject, float duration)
    {
        Vector3 startScale = rewardObject.transform.localScale;
        Vector3 targetScale = Vector3.one; // Zoom in scale
        Vector3 startPos = rewardObject.transform.position;
        Vector3 targetPos = rewardTargetPos.transform.position; // Target position for the reward

        AudioManager.instance.PlaySFX("RewardAppear");
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            rewardObject.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            yield return null; // Wait for the next frame
        }
        rewardObject.transform.localScale = targetScale; // Set the final scale
        rewardObject.transform.position = targetPos; // Set the final position
    }

    public IEnumerator HideOneReward(GameObject rewardObject, float duration, Vector3 targetScale, Transform parent)
    {
        Vector3 startScale = rewardObject.transform.localScale;
        Vector3 startPos = rewardObject.transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rewardObject.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            rewardObject.transform.position = Vector3.Lerp(startPos, parent.position, elapsedTime / duration);
            yield return null; // Wait for the next frame
        }
        rewardObject.transform.localScale = targetScale; // Set the final scale
        rewardObject.transform.position = parent.position; // Set the final position
        rewardObject.transform.SetParent(parent); // Set the parent to the target parent after the animation
    }

    public IEnumerator OpenChestBoxAnim(float time)
    {
        float countTimer = 0f;
        float elapsedTime = 0f;
        float duration = time/10;

        RectTransform chestBoxRect = chestBoxImage.GetComponent<RectTransform>();

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // Rotate the chest box image to create an opening animation by angling it slightly
            chestBoxRect.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -10, elapsedTime / (duration/2) ));

            countTimer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        chestBoxRect.localRotation = Quaternion.Euler(0, 0, -10); // Ensure the final rotation is set

        while (countTimer < time)
        {
            elapsedTime = 0f; // Reset elapsed time for the next iteration
            while(elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                // Rotate the chest box image to create an opening animation by angling it slightly
                chestBoxRect.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-10, 10, elapsedTime / duration));

                countTimer += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            chestBoxRect.localRotation = Quaternion.Euler(0, 0, 10); // Ensure the final rotation is set

            elapsedTime = 0f; // Reset elapsed time for the next iteration
            // Rotate back to the opposite angle 
            while(elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                chestBoxRect.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(10, -10, elapsedTime / duration));

                countTimer += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            chestBoxRect.localRotation = Quaternion.Euler(0, 0, -10); // Ensure the final rotation is set
        }

        chestBoxRect.localRotation = Quaternion.Euler(0, 0, 0); // Reset the rotation to the original position
        smokeChestBoxVFX.Play(); // Play the smoke effect when the chest box is opened
        chestBoxImage.sprite = chestBoxOpenedSprite; // Change the sprite to the open chest box sprite
        lightChestBoxVFX.Play(); // Play the light effect when the chest box is opened
        AudioManager.instance.PlaySFX("ChestOpen");
    }

    public void OnClickClaimButton()
    {
        if(isShowRewardRunning) return; // If the reward display is still running, do not allow claiming rewards

        pressCount = 0;

        openChestBoxPanel.SetActive(false); // Hide the chest box panel
        claimRewardButton.gameObject.SetActive(false); // Hide the claim reward button
        //Clear child in itemParent and currencyParent
        for (int i = itemParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(itemParent.transform.GetChild(i).gameObject);
        }

        for (int i = currencyParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(currencyParent.transform.GetChild(i).gameObject);
        }

        //Return the chest box to the begin form
        chestBoxImage.sprite = chestBoxClosedSprite;
        lightChestBoxVFX.Stop();
        chestBoxImage.gameObject.SetActive(false);

        //Currency anim
        if (isClaimCrystal)
        {
            StartCoroutine(CurrencyPanelZoomInAndZoomOut("crystal", 0.1f));
            isClaimCrystal = false;
        }
        if (isClaimStar)
        {
            StartCoroutine(CurrencyPanelZoomInAndZoomOut("star", 0.1f));
            isClaimStar = false;
        }
        if (isClaimCoin)
        {
            StartCoroutine(CurrencyPanelZoomInAndZoomOut("coin", 0.1f));
            isClaimCoin = false;
        }
    }

    #endregion
}
