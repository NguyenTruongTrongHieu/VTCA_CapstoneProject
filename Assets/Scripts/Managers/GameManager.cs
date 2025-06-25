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
    public string currentTurn = "";//"": not playing; "Player": player turn; "Enemy": enemy turn
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
}
