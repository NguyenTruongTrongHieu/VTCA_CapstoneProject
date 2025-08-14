using CartoonFX;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Serialization;
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

    public Text currentLevelDisplay;

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
    public Button ultimateButton;
    public Slider manaSlider;
    public float startPosYManaSlider;
    public Text currentStageProgressionDisplay;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public Button returnMenuButton;

    [Header("DamText Prefabs")]
    public GameObject damageText;
    [Tooltip("The width of the character in the damage text prefab, used for positioning the text correctly")]
    public float characterWidthDamText;

    [Header("CoinText prefabs")]
    public GameObject coinTextPrefab;
    public float characterWidthCoinText;

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
        // Set up the main menu
        SetUITextForUpgradeDamButton();
        SetUITextForUpgradeHealthButton();
        UpdateColorTextForUpgradeCost();
        UpdateCoinText();
        UpdateCrystalText();
        UpdateStarText();

        ShowMainMenuPanel();

        startPosYManaSlider = manaSlider.GetComponent<RectTransform>().anchoredPosition.y;

        // Display the current level in the main menu
        DisplayCurrentLevel();

        // Set up the character buttons
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

    // Update is called once per frame
    void Update()
    {

    }

    #region MENU
    public void OnclickStartButton()
    {
        GameManager.instance.currentGameState = GameState.Playing;
        GameManager.instance.currentTurn = "None"; // Set the current turn to None

        ShowInGamePanel();

        //StartCoroutine(
        //CameraManager.instance.SetScreenPosComposition(1f, true, -0.25f));
        StartCoroutine(
        CameraManager.instance.SetHardLookAt(1f, 'Z', 0.7f));
        PlayerUltimate.instance.AddUltimateToUltiButton(PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().id);
    }

    public void DisplayCurrentLevel()
    {
        if (LevelManager.instance.currentLevel != null)
        {
            currentLevelDisplay.text = $"Level {LevelManager.instance.currentLevel.index}";
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

    public IEnumerator CurrencyPanelZoomInAndZoomOut(string currency)
    {
        Transform targetObject;
        if(currency == "coin")
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
        while(elaspedTime < 0.05f)
        { 
            elaspedTime += Time.deltaTime;
            targetObject.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.2f);
            yield return null;
        }
        targetObject.localScale = targetScale; // Set the final scale

        elaspedTime = 0f;
        startScale = targetObject.localScale;
        targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Zoom in scale
        while (elaspedTime < 0.1f)
        {
            elaspedTime += Time.deltaTime;
            targetObject.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.2f);
            yield return null;
        }
        targetObject.localScale = targetScale; // Set the final scale

        elaspedTime = 0f;
        startScale = targetObject.localScale;
        targetScale = new Vector3(1f, 1f, 1f); // Reset to original scale
        while (elaspedTime < 0.05f)
        {
            elaspedTime += Time.deltaTime;
            targetObject.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.2f);
            yield return null;
        }
        targetObject.localScale = targetScale; // Set the final scale
    }

    public IEnumerator SpawnCoinPrefabAndMoveToCoinPanel(Transform startTransform, int coinAmount)
    {
        string text = NumberFomatter.FormatIntToString(coinAmount, 2);
        Vector3 startPos = Camera.main.WorldToScreenPoint(startTransform.position);

        GameObject coinTextObject = Instantiate(coinTextPrefab, startPos, Quaternion.identity, transform);
        Text coinText = coinTextObject.GetComponent<Text>();
        RectTransform rectTransformCoinText = coinTextObject.GetComponent<RectTransform>();

        // Set the text and text size
        coinText.text += text;
        Vector2 textSize = rectTransformCoinText.sizeDelta;
        textSize.x += characterWidthCoinText * text.Length;
        rectTransformCoinText.sizeDelta = textSize;

        // Animation Text ZoomOut and Move Up
        float elaspedTime = 0f;
        Vector3 startScale = rectTransformCoinText.localScale;
        Vector3 targetScale = Vector3.one; // Zoom in scale
        while (elaspedTime < 0.1f) // Run this animation for 0.1s
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
        Vector3 targetPos = coinPanel.transform.position;
        while (elaspedTime < 0.2f) // Run this animation for 0.2s
        {
            elaspedTime += Time.deltaTime;
            rectTransformCoinText.position = Vector3.Lerp(startPos, targetPos, elaspedTime / 0.2f);
            yield return null;
        }
        rectTransformCoinText.position = targetPos; // Set the final position
        Destroy(coinTextObject);
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
            if(SaveLoadManager.instance.currentLevelOfCurrentPlayer >= PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().bonusStatsLevel.Length)
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
        else if(playerStat.id == 2)
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
            foreach(var name in checkCharacter.ownedSkins)
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
        PlayerUltimate.instance.GetPlayerTransform(characterName, characterLevel, 0.75f);

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
        damageLevelText.text = $"Damage LV.{NumberFomatter.FormatIntToString(GameManager.instance.currentDamageLevel, 0)}";
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
        healthLevelText.text = $"Health LV.{NumberFomatter.FormatIntToString(GameManager.instance.currentHealthLevel, 0)}";
        basicHealthText.text = $"{NumberFomatter.FormatFloatToString(GameManager.instance.basicHealth, 2)}";
        costToUpgradeHealthText.text = $"{NumberFomatter.FormatFloatToString(GameManager.instance.healthCostToUpgrade, 2)}";//Đưa hàm tính giá tiền vào đây
    }
    #endregion

    #region IN GAME

    public void DisplayDamageText(Transform startTransform, Transform targetTransform, float dam)
    { 
        string text = NumberFomatter.FormatFloatToString(dam, 2);
        Vector3 startPos = Camera.main.WorldToScreenPoint(startTransform.position);

        GameObject damageTextObject = Instantiate(damageText, startPos, Quaternion.identity, transform);
        Text damText = damageTextObject.GetComponent<Text>();
        RectTransform rectTransformDamText = damageTextObject.GetComponent<RectTransform>();

        // Set the text and text size
        damText.text += text;
        Vector2 textSize = rectTransformDamText.sizeDelta;
        textSize.x += characterWidthDamText * text.Length;
        rectTransformDamText.sizeDelta = textSize;

        //Animation Text ZoomOut and Move Up
        StartCoroutine(DisplayText(startPos, Camera.main.WorldToScreenPoint(targetTransform.position), rectTransformDamText));
    }

    IEnumerator DisplayText(Vector3 startPos, Vector3 targetPos, RectTransform text)
    { 
        float elaspedTime = 0f;
        Vector3 startScale = text.localScale;
        Vector3 targetScale = Vector3.one;
        while (elaspedTime < 0.1f)//Chạy anim này trong vòng 0.1s
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
        Destroy(text.gameObject); // Xóa text sau khi hoàn thành
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

    #endregion

    #region SETTING
    public void OnCLickReturnMenuButton()
    { 
        SaveLoadManager.instance.loadingPanel.SetActive(true);

        GameManager.instance.currentGameState = GameState.MainMenu;
        GameManager.instance.currentTurn = ""; // Reset the current turn
        GameManager.instance.currentEnemyIndex = 0; // Reset the current enemy index

        GameManager.instance.StartCoroutine(GameManager.instance.LoadNewLevel());
    }
    #endregion

    #region SHOW PANEL

    public void ShowInGamePanel()
    {
        mainMenuPanel.SetActive(false);
        ultimateButton.gameObject.SetActive(false);
        manaSlider.gameObject.SetActive(true);
        inGamePanel.SetActive(true);
        //foreach (Image tabsButtons in tabsManager.tabButtons)
        //{
        //    tabsButtons.gameObject.SetActive(false); // Hide all tab buttons
        //}
    }

    public void ShowGameOverPanel(bool isPlayerWin)
    {
        gameOverPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        inGamePanel.SetActive(false);
    }

    public void ShowMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        inGamePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        //foreach (Image tabsButtons in tabsManager.tabButtons)
        //{
        //    tabsButtons.gameObject.SetActive(true); // Hide all tab buttons
        //}
    }

    public IEnumerator ShowPanelWithZoomInAnim(GameObject panel, float duration)
    { 
        float elapsedTime = 0f;
        Vector3 startScale = panel.transform.localScale;
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
        Vector3 targetScale = Vector3.zero; // Zoom out scale
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            panel.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            yield return null;
        }
        panel.transform.localScale = targetScale; // Set the final scale
        panel.SetActive(false); // Hide the panel after the animation
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
        StartCoroutine(ShowPanelWithZoomInAnim(BuyCharacterPanel, 0.2f));
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
        StartCoroutine(ShowPanelWithZoomInAnim(UpgradeCharacterPanel, 0.2f));
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
        StartCoroutine(ShowPanelWithZoomInAnim(WarningNotEnoughCostPanel, 0.2f));
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
    public void OnClickOverGameButton()
    {
        SaveLoadManager.instance.loadingPanel.SetActive(true);

        //Reset level or get new level
        if (GameManager.instance.currentTurn == "Win")
        {
            LevelManager.instance.SetNextLevel(); // Set the next level
        }
        else if (GameManager.instance.currentTurn == "Lose")
        {
        }

        //Basic reset
        GameManager.instance.currentGameState = GameState.MainMenu;
        GameManager.instance.currentTurn = ""; // Reset the current turn
        GameManager.instance.currentEnemyIndex = 0; // Reset the current enemy index

        GameManager.instance.StartCoroutine(GameManager.instance.LoadNewLevel());
    }
    #endregion
}
