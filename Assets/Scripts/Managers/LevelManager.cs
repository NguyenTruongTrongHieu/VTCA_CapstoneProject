using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Level currentLevel;//Level hien tai va cac enemy cua level do, duoc tham chieu tu scene
    public Level[] levels;// Danh sach cac level trong game, cac enemy duoc tham chieu tu asset

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //currentLevel = levels[0];
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemiesAtCurrentLevel()
    { 
        currentLevel.enemiesAtLevel.Clear(); // Clear the current level's enemies list before spawning new ones
        Level level = levels[currentLevel.index]; // Get the current level from the levels array
        //Debug.Log("Spawning enemies for level: " + level.index + " - " + level.sceneName);
        for (int i = 0; i < level.enemiesAtLevel.Count; i++)
        { 
            var enemy = Instantiate(level.enemiesAtLevel[i], 
                GameManager.instance.enemiesStartPosition[i], 
                Quaternion.Euler(GameManager.instance.enemiesStartRotation));

            currentLevel.enemiesAtLevel.Add(enemy); // Add the enemy to the current level's enemies list
        }
    }

    public void DeleteAllEnemy()
    {
        foreach (var enemy in currentLevel.enemiesAtLevel)
        {
            if (enemy == null || !enemy.activeSelf)
            {
                continue;
            }
            //Debug.Log("Destroying enemy: " + enemy.name);
            Destroy(enemy); // Destroy the enemy GameObject
        }
    }

    public void AddStateAndLockCellToCurrentLevel()
    { 
        currentLevel.lockCellInBoard = levels[currentLevel.index].lockCellInBoard;
        currentLevel.statesInBoard = levels[currentLevel.index].statesInBoard;
    }

    public void SetNextLevel()
    { 
        int currentLevelIndex = currentLevel.index;
        if (currentLevelIndex < levels.Length - 1)
        {
            currentLevel = new Level(currentLevelIndex + 1, levels[currentLevelIndex + 1].sceneName, levels[currentLevelIndex + 1].havingBoss, levels[currentLevelIndex + 1].rewardCoin); // Set the next level
            AddStateAndLockCellToCurrentLevel(); // Add states and lock cells to the new level
            SaveLoadManager.instance.currentLevelIndex = currentLevel.index; // Save the new level index

            if (MissionsManager._instance.missions != null)
            {
                MissionsManager._instance.ReachLevel(currentLevel.index); // Update missions with the new level
            }
        }
        else
        {
            Debug.Log("No more levels available.");
            //currentLevel = new Level(currentLevelIndex, levels[currentLevelIndex - 1].sceneName);
            //AddStateAndLockCellToCurrentLevel();
        }
    }
}
