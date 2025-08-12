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
        LevelManager.instance.currentLevel = new Level(currentLevelIndex, LevelManager.instance.levels[currentLevelIndex - 1].sceneName);
        LevelManager.instance.AddStateAndLockCellToCurrentLevel();

        GameManager.instance.currentDamageLevel = currentBasicDamageLevel;
        GameManager.instance.currentHealthLevel = currentBasicHealthLevel;
        GameManager.instance.SetUpBasicDamAndHP();

        CurrencyManager.instance.coins = currentCoin;
        CurrencyManager.instance.crystals = currentCrystal;
        CurrencyManager.instance.stars = currentStar;

        StartCoroutine(LoadingSceneAsync());
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

        currentPlayerName = "Player1_ Skin1";
        currentLevelOfCurrentPlayer = 1;

        currentCoin = 0;
        currentCrystal = 0;
        currentStar = 0;

        ownedCharacters = new List<OwnedCharacter>
        {
            new OwnedCharacter(0, 2, new List<string>{ "Player1", "Player1_ Skin1"}),
            new OwnedCharacter(2, 1, "Player3"),
        };
    }

    public IEnumerator LoadingSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(LevelManager.instance.currentLevel.sceneName);
        loadingPanel.SetActive(true);

        while (!asyncOperation.isDone)
        { 
            yield return null; // Wait for the next frame
        }
        loadingPanel.SetActive(false);
    }
}
