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
                }
            }
            else if (isPlayerTurn)
            {
                isPlayerTurn = false; // Enemy's turn
                //Disable input
            }
        }
    }

    public bool CheckIfAllEnemiesDead()
    { 
        bool result = true;

        foreach (var enemy in LevelManager.instance.currentLevel.enemiesAtLevel)
        {
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
