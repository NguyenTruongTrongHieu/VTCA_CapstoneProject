using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState currentGameState = GameState.MainMenu;

    [Header("MainMenu")]
    public float basicDamage = 10f; // Basic damage for the player
    public float basicHealth = 100f; // Basic health for the player

    [Header("Playing")]
    public string currentTurn = "";//"": not playing; "Player": player turn; "Enemy": enemy turn; "None": not do attack; "Win": all enemies die; "Lose": player die
    private bool isPlayerTurn = false; // True if it's player's turn, false if it's enemy's turn
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
}
