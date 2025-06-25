using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Main menu")]
    public Button playButton;
    public GameObject mainMenuPanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

    public void OnclickStartButton()
    {
        GameManager.instance.currentGameState = GameState.Playing;
        mainMenuPanel.SetActive(false);
        StartCoroutine(
        CameraManager.instance.SetScreenPosComposition(1f, true, -0.25f));
    }
}
