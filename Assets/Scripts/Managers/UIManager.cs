using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Main menu")]
    public Button playButton;
    public GameObject mainMenuPanel;

    [Header("In Game")]
    public GameObject inGamePanel;
    public GameObject gameBoard;
    public GameObject disableMatching;
    public Button ultimateButton;
    public Slider manaSlider;
    public GameObject targetPos;//Test

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenuPanel.SetActive(true);
        inGamePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnclickStartButton()
    {
        GameManager.instance.currentGameState = GameState.Playing;
        GameManager.instance.currentTurn = "None"; // Set the current turn to None

        mainMenuPanel.SetActive(false);
        inGamePanel.SetActive(true);

        //StartCoroutine(
        //CameraManager.instance.SetScreenPosComposition(1f, true, -0.25f));
        StartCoroutine(
        CameraManager.instance.SetHardLookAt(1f, 'Z', 0.7f));
        PlayerUltimate.instance.AddUltimateToUltiButton(PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().id);
    }
}
