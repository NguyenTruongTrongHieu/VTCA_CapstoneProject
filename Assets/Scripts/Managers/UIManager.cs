using CartoonFX;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TabsManager tabsManager;

   
    [Header("Currency")]
    public GameObject coinPanel;
    public Text coinText;

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
        UpdateCoinText();

        ShowMainMenuPanel();

        startPosYManaSlider = manaSlider.GetComponent<RectTransform>().anchoredPosition.y;

        // Display the current level in the main menu
        DisplayCurrentLevel();
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

    public IEnumerator CoinPanelZoomInAndZoomOut()
    { 
        float elaspedTime = 0f;
        Vector3 startScale = coinPanel.transform.localScale;
        Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f); // Zoom out scale
        while(elaspedTime < 0.05f)
        { 
            elaspedTime += Time.deltaTime;
            coinPanel.transform.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.2f);
            yield return null;
        }
        coinPanel.transform.localScale = targetScale; // Set the final scale

        elaspedTime = 0f;
        startScale = coinPanel.transform.localScale;
        targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Zoom in scale
        while (elaspedTime < 0.1f)
        {
            elaspedTime += Time.deltaTime;
            coinPanel.transform.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.2f);
            yield return null;
        }
        coinPanel.transform.localScale = targetScale; // Set the final scale

        elaspedTime = 0f;
        startScale = coinPanel.transform.localScale;
        targetScale = new Vector3(1f, 1f, 1f); // Reset to original scale
        while (elaspedTime < 0.05f)
        {
            elaspedTime += Time.deltaTime;
            coinPanel.transform.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.2f);
            yield return null;
        }
        coinPanel.transform.localScale = targetScale; // Set the final scale
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

    #endregion

    #region UPGRADE
    public void OnClickUpgradeDamButton()
    {
        //Increase the damage level
        GameManager.instance.UpgradeDam();
        //Update Player ultimate
        PlayerUltimate.instance.SetUpBaseStatForPlayer();

        //Set UIButton
        SetUITextForUpgradeDamButton();
    }

    public void OnClickUpgradeHealthButton()
    {
        //Increase the health level
        GameManager.instance.UpgradeHealth();
        //Update Player ultimate
        PlayerUltimate.instance.SetUpBaseStatForPlayer();

        //Set UIButton
        SetUITextForUpgradeHealthButton();
    }

    public void SetUITextForUpgradeDamButton()
    {
        damageLevelText.text = $"Damage LV.{GameManager.instance.currentDamageLevel}";
        basicDamageText.text = $"{GameManager.instance.basicDamage}";
        costToUpgradeDamText.text = $"0";//Đưa hàm tính giá tiền vào đây
    }

    public void SetUITextForUpgradeHealthButton()
    {
        healthLevelText.text = $"Health LV.{GameManager.instance.currentHealthLevel}";
        basicHealthText.text = $"{GameManager.instance.basicHealth}";
        costToUpgradeHealthText.text = $"0";//Đưa hàm tính giá tiền vào đây
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
        GameBoard.Instance.ResetBoard();
        GameBoard.Instance.InitializeFood(LevelManager.instance.currentLevel.statesInBoard, LevelManager.instance.currentLevel.lockCellInBoard);

        LevelManager.instance.DeleteAllEnemy();
        LevelManager.instance.SpawnEnemiesAtCurrentLevel();
        ultimateButton.onClick.RemoveAllListeners();
        PlayerUltimate.instance.ResetPlayer();

        SaveLoadManager.instance.loadingPanel.SetActive(false);
    }
    #endregion

    #region SHOW PANEL

    public void ShowInGamePanel()
    {
        mainMenuPanel.SetActive(false);
        ultimateButton.gameObject.SetActive(false);
        manaSlider.gameObject.SetActive(true);
        inGamePanel.SetActive(true);
        foreach (Image tabsButtons in tabsManager.tabButtons)
        {
            tabsButtons.gameObject.SetActive(false); // Hide all tab buttons
        }
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

        foreach (Image tabsButtons in tabsManager.tabButtons)
        {
            tabsButtons.gameObject.SetActive(true); // Hide all tab buttons
        }
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
