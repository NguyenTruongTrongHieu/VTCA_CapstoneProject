using CartoonFX;
using System.Collections;
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

    [Header("In Game")]
    public GameObject inGamePanel;
    public GameObject gameBoard;
    public GameObject disableMatching;
    public Button ultimateButton;
    public Slider manaSlider;
    public GameObject targetPos;//Test

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

        mainMenuPanel.SetActive(false);
        inGamePanel.SetActive(true);

        //StartCoroutine(
        //CameraManager.instance.SetScreenPosComposition(1f, true, -0.25f));
        StartCoroutine(
        CameraManager.instance.SetHardLookAt(1f, 'Z', 0.7f));
        PlayerUltimate.instance.AddUltimateToUltiButton(PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().id);
        foreach (Image tabsButtons in tabsManager.tabButtons)
        {
            tabsButtons.gameObject.SetActive(false); // Hide all tab buttons
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

        //Set UIButton
        SetUITextForUpgradeDamButton();
    }

    public void OnClickUpgradeHealthButton()
    {
        //Increase the health level
        GameManager.instance.UpgradeHealth();

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
