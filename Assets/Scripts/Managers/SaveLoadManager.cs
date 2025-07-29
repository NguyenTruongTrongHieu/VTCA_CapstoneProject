using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public int currentDiamond;
    public int currentGoldKey;
    public List<string> unlockPlayerAndSkin;//saving name PlayerPrefab
    public Dictionary<int, int> unlockPlayerAndPlayerCurrentLevel;//saving id player and current level of that player

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
        StartCoroutine(LoadingSceneAsync());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadDataWithPlayerPref()
    {
        currentLevelIndex = 2;
        currentPlayerName = "Player1";
        currentLevelOfCurrentPlayer = 1;
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
