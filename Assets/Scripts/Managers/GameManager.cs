using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    const int BASEVALUE_DAMAGE = 10; // Increment value for damage level
    const int BASEVALUE_HEALTH = 100; // Increment value for health level
    const int BASECOST_DAMAGE = 30; // Base cost for upgrading damage
    const int BASECOST_HEALTH = 20; // Base cost for upgrading health

    public static GameManager instance;

    public GameState currentGameState = GameState.MainMenu;

    [Header("MainMenu")]
    public int currentDamageLevel;
    public int currentHealthLevel;
    public float basicDamage = 10f; // Basic damage for the player
    public int damCostToUpgrade = 30; // Cost to upgrade basic damage
    public float basicHealth = 100f; // Basic health for the player
    public int healthCostToUpgrade = 20; // Cost to upgrade basic health

    [Header("Playing")]
    public string currentTurn = "";//"": not playing; "Player": player turn; "Enemy": enemy turn; "None": not do attack; "Win": all enemies die; "Lose": player die
    private bool isPlayerTurn = false; // True if it's player's turn, false if it's enemy's turn
    public int multipleScoreForPlayerHit = 1; // Score multiplier for player hits

    public int currentEnemyIndex = 0; // Index of the current enemy in the level
    public List<Vector3> enemiesStartPosition = new List<Vector3>(); // List to store the start positions of enemies
    public Vector3 enemiesStartRotation;
    public Vector3 enemiesEndRotation;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            currentGameState = GameState.MainMenu;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //Enable and disable input based on the current game state and turn
        if (currentGameState == GameState.Playing)
        {
            if (currentTurn == "Player")
            {
                if (!isPlayerTurn)
                {
                    isPlayerTurn = true; // Player's turn
                    //Enable input
                    UIManager.instance.disableMatching.SetActive(false); // Disable matching UI during player's turn
                    UIManager.instance.ultimateButton.interactable = true; // Disable ultimate button during player's turn
                }
            }
            else //if (isPlayerTurn)
            {
                isPlayerTurn = false; // Enemy's turn
                //Disable input
                UIManager.instance.disableMatching.SetActive(true); // Enable matching UI during enemy's turn
                UIManager.instance.ultimateButton.interactable = false;
            }
        }
    }

    public void GoToNextEnemy()
    {
        UIManager.instance.SetCurrentProgress(true);
        currentEnemyIndex++;
    }

    public bool CheckIfAllEnemiesDead()
    {
        bool result = true;

        foreach (var enemy in LevelManager.instance.currentLevel.enemiesAtLevel)
        {
            if (enemy == null) continue; // Skip if enemy is null

            var enemyStat = enemy.GetComponent<EnemyStat>();
            if (enemyStat != null && !enemyStat.CheckIfObjectDead())
            {
                result = false; // If any enemy is not dead, return false
                break; // No need to check further
            }
        }

        return result;
    }

    public bool CheckIfCurrentEnemyDead()
    {
        bool result = false;
        // Check if the current enemy is dead
        var enemy = LevelManager.instance.currentLevel.enemiesAtLevel[currentEnemyIndex];
        if (enemy == null) return true; // If the enemy is null, return true

        var enemyStat = LevelManager.instance.currentLevel.enemiesAtLevel[currentEnemyIndex].GetComponent<EnemyStat>();
        if (enemyStat != null)
        {
            if (enemyStat.CheckIfObjectDead())
            {
                result = true; // Current enemy is dead
            }
        }
        // This method should be called after the enemy takes damage
        // Return true if the enemy is dead, false otherwise
        return result; // Placeholder implementation
    }

    public void SetUpBasicDamAndHP()
    {
        SetUpBasicDam();
        GetCostToUpgradeBasicDam();
        SetUpBasicHP();
        GetCostToUpgradeBasicHP();
    }

    public void SetUpBasicDam()
    {
        int incrementDamage = 0;

        if (currentDamageLevel < 10)
        {
            incrementDamage = 2;
        }
        else
        { 
            incrementDamage = 5;
        }

        basicDamage = BASEVALUE_DAMAGE + ((currentDamageLevel - 1) * incrementDamage);
    }

    public int GetCostToUpgradeBasicDam()
    {
        float growthFactor = 1.15f;

        if (currentDamageLevel <= 1)
        {
            damCostToUpgrade = BASECOST_DAMAGE;
            return damCostToUpgrade;
        }
        else if (currentDamageLevel < 5)
        {
            growthFactor = 1.15f;
        }
        else if (currentDamageLevel < 10)
        {
            growthFactor = 1.3f; // Increase growth factor for levels 1-9
        }
        else if (currentDamageLevel < 20)
        {
            growthFactor = 1.5f; // Decrease growth factor for levels 10 and above
        }

        damCostToUpgrade = (int)(BASECOST_DAMAGE * Mathf.Pow(growthFactor, currentDamageLevel - 0));
        return damCostToUpgrade;
    }

    public void SetUpBasicHP()
    {
        int incrementHealth = 0;

        if (currentHealthLevel < 10)
        {
            incrementHealth = 5;
        }
        else
        { 
            incrementHealth = 10;
        }

        basicHealth = BASEVALUE_HEALTH + ((currentHealthLevel - 1) * incrementHealth);
    }

    public int GetCostToUpgradeBasicHP()
    {
        float growthFactor = 1.15f;

        if (currentHealthLevel <= 1)
        {
            healthCostToUpgrade = BASECOST_HEALTH;
            return healthCostToUpgrade;
        }
        else if (currentHealthLevel < 5)
        {
            growthFactor = 1.15f;
        }
        else if (currentHealthLevel < 10)
        {
            growthFactor = 1.3f; // Increase growth factor for levels 1-9
        }
        else if (currentHealthLevel < 20)
        {
            growthFactor = 1.5f; // Decrease growth factor for levels 10 and above
        }

        healthCostToUpgrade = (int)(BASECOST_HEALTH * Mathf.Pow(growthFactor, currentHealthLevel - 0));
        return healthCostToUpgrade;
    }

    public void UpgradeDam()
    { 
        currentDamageLevel++;
        SaveLoadManager.instance.currentBasicDamageLevel = currentDamageLevel;
        SetUpBasicDam();
        GetCostToUpgradeBasicDam();

        if (MissionsManager._instance.missions != null)
        {
           MissionsManager._instance.UpgradeDamageStats();
        }

        PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().SetUpStatAndSlider();
    }

    public void UpgradeHealth()
    { 
        currentHealthLevel++;
        SaveLoadManager.instance.currentBasicHealthLevel = currentHealthLevel;
        SetUpBasicHP();
        GetCostToUpgradeBasicHP();

        if (MissionsManager._instance.missions != null)
        {
            MissionsManager._instance.UpgradeHealthStats();
        }

        PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().SetUpStatAndSlider();
    }

    public IEnumerator LoadNewLevel()
    {
        SaveLoadManager.instance.loadingPanel.SetActive(true);
        yield return StartCoroutine(SaveLoadManager.instance.MoveUpLoadingPanel());

        //Reset level or get new level
        if (currentTurn == "Win")
        {
            LevelManager.instance.SetNextLevel(); // Set the next level
        }
        else if (currentTurn == "Lose")
        {
        }

        //Basic reset
        currentGameState = GameState.MainMenu;
        currentTurn = ""; // Reset the current turn
        currentEnemyIndex = 0; // Reset the current enemy index

        //LoadScene
        if (LevelManager.instance.currentLevel.sceneName != SceneManager.GetActiveScene().name)
        {
            //AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(LevelManager.instance.currentLevel.sceneName);

            //while (!asyncOperation.isDone)
            //{
            //    yield return null; // Wait for the next frame
            //}
            yield return StartCoroutine(SaveLoadManager.instance.LoadingSceneAsync(false, 0.75f));
            //StartCoroutine(SaveLoadManager.instance.WaitingLoadingScene(0.75f));
        }
        else
        {
            //Reset game board
            StartCoroutine(SaveLoadManager.instance.WaitingLoadingScene(0.75f));
            GameBoard.Instance.ResetBoard();
            GameBoard.Instance.InitializeFood(LevelManager.instance.currentLevel.statesInBoard, LevelManager.instance.currentLevel.lockCellInBoard);

            UIManager.instance.DisplayCurrentLevel();
        }

        //Reset enemies and player
        LevelManager.instance.DeleteAllEnemy();
        LevelManager.instance.SpawnEnemiesAtCurrentLevel();
        UIManager.instance.ultimateButton.onClick.RemoveAllListeners(); // Remove all listeners from the ultimate button
        PlayerUltimate.instance.ResetPlayer();

        UIManager.instance.UpdateColorTextForUpgradeCost();
        UIManager.instance.ShowMainMenuPanel();

        //SaveLoadManager.instance.loadingPanel.SetActive(false);
    }

    //public void Vibrate()
    //{ 
    //    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
    //    {
    //        Handheld.Vibrate(); // Vibrate the device
    //    }
    //}
}
