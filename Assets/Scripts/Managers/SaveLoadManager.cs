using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OwnedCharacter
{
    public int characterID;
    public int currentLevel;
    public List<string> ownedSkins = new List<string>();

    public OwnedCharacter(int id, string skinName)
    { 
        characterID = id;
        currentLevel = 1; // Default level is 1
        if (ownedSkins == null)
        {
            ownedSkins = new List<string> { skinName };
        }
        else
        {
            ownedSkins.Add(skinName);
        }
    }

    public OwnedCharacter(int id, int level, string skinName)
    {
        characterID = id;
        currentLevel = level;
        if (ownedSkins == null)
        {
            ownedSkins = new List<string> { skinName };
        }
        else
        {
            ownedSkins.Add(skinName);
        }
    }

    public OwnedCharacter(int id, int level, List<string> skins)
    {
        characterID = id;
        currentLevel = level;
        ownedSkins = skins;
    }

    public OwnedCharacter(int id, List<string> skins)
    {
        characterID = id;
        currentLevel = 1;
        ownedSkins = new List<string>();
    }
}

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;

    [Header("Saving")]
    public int currentLevelIndex;
    public int currentBasicHealthLevel;
    public int currentBasicDamageLevel;

    public string currentPlayerName;
    public int currentLevelOfCurrentPlayer;

    public int currentCoin;
    public int currentCrystal;
    public int currentStar;
    public List<OwnedCharacter> ownedCharacters = new List<OwnedCharacter>();

    [Header("Loading UI")]
    public GameObject loadingPanel;
    public Slider loadingSlider;
    public Image backGroundImage;
    public Sprite[] backGround;
    public Text loadingText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadDataWithPlayerPref();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelManager.instance.currentLevel = new Level(currentLevelIndex, LevelManager.instance.levels[currentLevelIndex - 1].sceneName, 
            LevelManager.instance.levels[currentLevelIndex - 1].havingBoss, LevelManager.instance.levels[currentLevelIndex - 1].rewardCoin);
        LevelManager.instance.AddStateAndLockCellToCurrentLevel();

        GameManager.instance.currentDamageLevel = currentBasicDamageLevel;
        GameManager.instance.currentHealthLevel = currentBasicHealthLevel;
        GameManager.instance.SetUpBasicDamAndHP();

        CurrencyManager.instance.coins = currentCoin;
        CurrencyManager.instance.crystals = currentCrystal;
        CurrencyManager.instance.stars = currentStar;

        StartCoroutine(LoadingSceneAsync(true, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadDataWithPlayerPref()
    {
        currentLevelIndex = 1;
        currentBasicDamageLevel = 1;
        currentBasicHealthLevel = 1;

        currentPlayerName = "Player2";
        currentLevelOfCurrentPlayer = 2;

        currentCoin = 109000;
        currentCrystal = 0;
        currentStar = 1000;

        ownedCharacters = new List<OwnedCharacter>
        {
            new OwnedCharacter(1, 2, new List<string>{ "Player2"}),
        };
    }

    public IEnumerator LoadingSceneAsync(bool isSetActiveLoadingPanel, float waitingTime)
    {
        backGroundImage.sprite = backGround[Random.Range(0, backGround.Length)];
        loadingText.text = "Loading...";
        loadingSlider.value = 0f; // Reset the slider value to 0
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(LevelManager.instance.currentLevel.sceneName);
        if (isSetActiveLoadingPanel)
        {
            loadingPanel.SetActive(true);
        }

        while (!asyncOperation.isDone)
        { 
            loadingSlider.value = asyncOperation.progress;
            yield return null; // Wait for the next frame
        }
        loadingSlider.value = 1f; // Ensure the slider is full when loading is complete

        if(waitingTime > 0)
        {
            StartCoroutine(WaitingLoadingScene(waitingTime));
        }
        else
        {
            if (isSetActiveLoadingPanel)
                loadingPanel.SetActive(false);

            loadingSlider.value = 0f;
        }
    }

    public IEnumerator WaitingLoadingScene(float waitingTime)
    {
        loadingText.text = "Let's go!";
        float elapsedTime = 0f;
        while (elapsedTime < waitingTime)
        {
            elapsedTime += Time.deltaTime;
            loadingSlider.value = Mathf.Lerp(0f, 1f, elapsedTime / waitingTime);
            yield return null; // Wait for the next frame
        }
        yield return new WaitForSeconds(0.3f);

        loadingPanel.SetActive(false);
        loadingSlider.value = 0f;
    }
}
