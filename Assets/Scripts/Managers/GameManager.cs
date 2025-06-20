using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public GameState currentGameState = GameState.MainMenu;

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
