using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using Assets.SimpleLocalization.Scripts;


public class OwnedCharacter
{
    public int characterID;
    public int currentLevel;
    public List<string> ownedSkins = new List<string>();

    public OwnedCharacter()
    {
        characterID = 0;
        currentLevel = 1;
        ownedSkins = new List<string>();
    }

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
    private bool isDataLoaded = false;

    [Header("Saving")]
    public int currentLevelIndex;
    public int currentBasicHealthLevel;
    public int currentBasicDamageLevel;

    public string currentPlayerName;
    public int currentLevelOfCurrentPlayer;

    public int currentCoin;
    public int currentCrystal;
    public int currentStar;

    public Mission[] missionsToSave;
    public bool isOpenedChestAtMission;

    public List<OwnedCharacter> ownedCharacters = new List<OwnedCharacter>();

    //Setting
    [Space]
    public bool isSFXOn = true;
    public bool isMusicOn = true;

    [Header("Loading UI")]
    public GameObject loadingPanel;
    private Vector2 originalOffsetMinLoadingPanel;
    private Vector2 originalOffsetMaxLoadingPanel;
    public Slider loadingSlider;
    public Image backGroundImage;
    public Sprite[] backGround;
    public Text loadingText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //PlayerPrefs.DeleteAll();
            isDataLoaded = LoadDataWithPlayerPref();
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
        originalOffsetMinLoadingPanel = loadingPanel.GetComponent<RectTransform>().offsetMin;
        originalOffsetMaxLoadingPanel = loadingPanel.GetComponent<RectTransform>().offsetMax;

        LevelManager.instance.currentLevel = new Level(currentLevelIndex, LevelManager.instance.levels[currentLevelIndex].sceneName, 
            LevelManager.instance.levels[currentLevelIndex].havingBoss, LevelManager.instance.levels[currentLevelIndex].rewardCoin);
        LevelManager.instance.AddStateAndLockCellToCurrentLevel();

        GameManager.instance.currentDamageLevel = currentBasicDamageLevel;
        GameManager.instance.currentHealthLevel = currentBasicHealthLevel;
        GameManager.instance.SetUpBasicDamAndHP();

        CurrencyManager.instance.coins = currentCoin;
        CurrencyManager.instance.crystals = currentCrystal;
        CurrencyManager.instance.stars = currentStar;

        if (!isDataLoaded)
        {
            //Setup mission here
            MissionsManager._instance.SetUpMissionsInfo();
        }
        else
        { 
            MissionsManager._instance.missions = missionsToSave;
            int missionsCompleted = 0;
            foreach (Mission mission in MissionsManager._instance.missions)
            {
                if (mission.isCompleted)
                {
                    missionsCompleted++;
                }
            }
            MissionsManager._instance.missionCompletedCount = missionsCompleted;
            MissionsManager._instance.isOpendChest = isOpenedChestAtMission;
            Timer.Instance.TimeSetup();

            if (Timer.Instance.GetRemainingTime() <= 0)
            {
                Timer.Instance.ResetMissionTime();
                MissionsManager._instance.SetUpMissionsInfo();
            }
        }

        AudioManager.instance.PreloadAllAudio();

        StartCoroutine(LoadingSceneAsync(true, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool LoadDataWithPlayerPref()//return true if can load, false if need to set default data
    {
        //Load owned characters first
        bool isFileExist = LoadDataWithFile("OwnedCharaters");
        bool result = false;

        if (!PlayerPrefs.HasKey("CurrentLevelIndex") || !isFileExist)//!PlayerPrefs.HasKey("CurrentLevelIndex") || !System.IO.File.Exists("OwnedCharaters")
        {
            currentLevelIndex = 0;
            currentBasicDamageLevel = 1;
            currentBasicHealthLevel = 1;

            currentPlayerName = "Luthor";
            currentLevelOfCurrentPlayer = 1;

            currentCoin = 0;
            currentCrystal = 0;
            currentStar = 0;

            ownedCharacters = new List<OwnedCharacter>
            {
                new OwnedCharacter(0, 1, new List<string>{ "Luthor"}),
            };

            //Set up missions
            isOpenedChestAtMission = false;
        }
        else//if hasKey
        {
            currentLevelIndex = PlayerPrefs.GetInt("CurrentLevelIndex");
            currentBasicDamageLevel = PlayerPrefs.GetInt("CurrentBasicDamageLevel");
            currentBasicHealthLevel = PlayerPrefs.GetInt("CurrentBasicHealthLevel");

            currentPlayerName = PlayerPrefs.GetString("CurrentPlayerName");
            currentLevelOfCurrentPlayer = PlayerPrefs.GetInt("CurrentLevelOfCurrentPlayer");
            //Check if currentPlayerName isn't in the ownedCharacters
            bool isPlayerNameExist = false;
            foreach (OwnedCharacter character in ownedCharacters)
            {
                if (character.ownedSkins.Contains(currentPlayerName))
                {
                    isPlayerNameExist = true;
                    break;
                }
            }
            if (!isPlayerNameExist)
            {
                //Current player name not found in owned characters, resetting to default.
                Debug.Log("Current player name not found in owned characters, resetting to default.");
                currentPlayerName = ownedCharacters[0].ownedSkins[0];
                currentLevelOfCurrentPlayer = ownedCharacters[0].currentLevel;
            }

            currentCoin = PlayerPrefs.GetInt("CurrentCoin");
            currentCrystal = PlayerPrefs.GetInt("CurrentCrystal");
            currentStar = PlayerPrefs.GetInt("CurrentStar");

            string missionsJson = PlayerPrefs.GetString("Missions");
            missionsToSave = JsonConvert.DeserializeObject<Mission[]>(missionsJson);
            isOpenedChestAtMission = PlayerPrefs.GetInt("IsOpenedChestAtMission") == 1;


            result = true;
        }

        //load settings
        isSFXOn = PlayerPrefs.GetInt("IsSFXOn", 1) == 0 ? false : true;
        isMusicOn = PlayerPrefs.GetInt("IsMusicOn", 1) == 0 ? false : true;

        return result;
    }

    public void SaveDataWithPlayerPref()
    { 
        PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex);
        PlayerPrefs.SetInt("CurrentBasicDamageLevel", currentBasicDamageLevel);
        PlayerPrefs.SetInt("CurrentBasicHealthLevel", currentBasicHealthLevel);

        PlayerPrefs.SetString("CurrentPlayerName", currentPlayerName);
        PlayerPrefs.SetInt("CurrentLevelOfCurrentPlayer", currentLevelOfCurrentPlayer);

        PlayerPrefs.SetInt("CurrentCoin", currentCoin);
        PlayerPrefs.SetInt("CurrentCrystal", currentCrystal);
        PlayerPrefs.SetInt("CurrentStar", currentStar);

        //Save missions
        string missionsJson = JsonConvert.SerializeObject(MissionsManager._instance.missions);
        PlayerPrefs.SetString("Missions", missionsJson);
        //isOpenedChestAtMission = MissionsManager._instance.isOpendChest;
        PlayerPrefs.SetInt("IsOpenedChestAtMission", isOpenedChestAtMission ? 1 : 0);

        //Save owned characters
        SaveDataWithFile("OwnedCharaters");

        //Save settings
        //PlayerPrefs.SetInt("IsSFXOn", !AudioManager.instance.sfxSource.mute ? 1 : 0);
        //PlayerPrefs.SetInt("IsMusicOn", !AudioManager.instance.musicSource.mute ? 1 : 0);

        //Save mission time
        Timer.Instance.SaveData();

        PlayerPrefs.Save();
    }

    #region USE FOR OWNED CHARACTERS
    public void SaveDataWithFile(string fileName)
    { 
        string path = Application.persistentDataPath + "/" + fileName + ".json";

        //Save owned characters
        string ownedCharacterJson = JsonConvert.SerializeObject(ownedCharacters);
        System.IO.File.WriteAllText(path, ownedCharacterJson);
    }

    public bool LoadDataWithFile(string fileName)//return true if file exists
    { 
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        bool result = false;
        if (System.IO.File.Exists(path))
        { 
            result = true;
            string ownedCharacterJson = System.IO.File.ReadAllText(path);
            ownedCharacters = JsonConvert.DeserializeObject<List<OwnedCharacter>>(ownedCharacterJson);
        }
        else
        {
            Debug.LogWarning("File not found: " + path);
        }
        return result;
    }
    #endregion

    #region ON PAUSE AND QUIT GAME
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveDataWithPlayerPref();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveDataWithPlayerPref();
        }
    }

    private void OnApplicationQuit()
    {
        SaveDataWithPlayerPref();
    }
    #endregion

    public IEnumerator LoadingSceneAsync(bool isSetActiveLoadingPanel, float waitingTime)
    {
        backGroundImage.sprite = backGround[Random.Range(0, backGround.Length)];

        if (LocalizationManager.Language == "Vietnamese")
        {
            loadingText.text = "Đang tải..."; 
        }
        else
        {
            loadingText.text = "Loading...";
        }

        loadingSlider.value = 0f; // Reset the slider value to 0
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(LevelManager.instance.currentLevel.sceneName);
        if (isSetActiveLoadingPanel)
        {
            ResetLoadingPanelPos();
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
            {
                yield return StartCoroutine(MoveDownLoadingPanel());
                loadingPanel.SetActive(false);
            }

            loadingSlider.value = 0f;
        }
    }

    public IEnumerator WaitingLoadingScene(float waitingTime)
    {
        if (LocalizationManager.Language == "Vietnamese")
        {
            loadingText.text = "Đi nào!";
        }
        else
        {
            loadingText.text = "Let's go!";
        }
        float elapsedTime = 0f;
        while (elapsedTime < waitingTime)
        {
            elapsedTime += Time.deltaTime;
            loadingSlider.value = Mathf.Lerp(0f, 1f, elapsedTime / waitingTime);
            yield return null; // Wait for the next frame
        }
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(MoveDownLoadingPanel());
        loadingPanel.SetActive(false);
        loadingSlider.value = 0f;
    }

    public IEnumerator MoveDownLoadingPanel()
    {
        yield return StartCoroutine(UIManager.instance.SlidePanel(loadingPanel.GetComponent<RectTransform>(), originalOffsetMinLoadingPanel,
            originalOffsetMaxLoadingPanel, originalOffsetMinLoadingPanel - new Vector2(0, 1920f), 
            originalOffsetMaxLoadingPanel - new Vector2(0, 1920f), 0.5f));
    }

    public IEnumerator MoveUpLoadingPanel()
    {
        loadingText.text = "";

        yield return StartCoroutine(UIManager.instance.SlidePanel(loadingPanel.GetComponent<RectTransform>(), loadingPanel.GetComponent<RectTransform>().offsetMin,
            loadingPanel.GetComponent<RectTransform>().offsetMax, originalOffsetMinLoadingPanel, 
            originalOffsetMaxLoadingPanel, 0.5f));
    }

    public void ResetLoadingPanelPos()
    {
        //Reset the loading panel position
        loadingPanel.GetComponent<RectTransform>().offsetMin = originalOffsetMinLoadingPanel;
        loadingPanel.GetComponent<RectTransform>().offsetMax = originalOffsetMaxLoadingPanel;
    }
}
