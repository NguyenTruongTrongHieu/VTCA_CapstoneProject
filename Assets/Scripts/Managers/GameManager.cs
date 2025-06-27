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
}
