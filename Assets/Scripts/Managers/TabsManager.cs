using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    public TabsManager charactersTab; // Singleton instance

    [SerializeField] private bool isTurnOnMenuAtStart;
    public GameObject[] tabs; // Array of tab GameObjects
    public Image[] tabButtons; // Array of tab button Images
    public Sprite inActiveTabBG, activeTabBG; // Sprites for active and inactive tab backgrounds
    public Vector2 inActiveButtonSize, activeButtonSize; // Sizes for active and inactive buttons


    public void SwitchToTabs(int TabID)
    {
            foreach (GameObject tab in tabs)
            {
                tab.SetActive(false); // Deactivate all tabs
            }
            tabs[TabID].SetActive(true); // Activate the selected tab

            foreach (Image image in tabButtons)
            {
                image.sprite = inActiveTabBG; // Set all tab buttons to inactive background
                image.rectTransform.sizeDelta = inActiveButtonSize; // Set all tab buttons to inactive size
            }

            tabButtons[TabID].sprite = activeTabBG; // Set the selected tab button to active background
            tabButtons[TabID].rectTransform.sizeDelta = activeButtonSize; // Set the selected tab button to active size

        if (TabID == 2)
        {
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetVerticalFOV(50f, 0.3f));
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetHardLookAt(3.5f, 'X', -1.25f));
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetHardLookAt(3.5f, 'Y', -1f));
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetFollowOffset(0.3f, 'X', 1.25f));
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetFollowOffset(0.3f, 'Y', 1f));
            charactersTab.TurnOnCharacterTab(0); // Activate the character tab
            UIManager.instance.SetUIInfoCurrentPlayer();
        }
        else
        {
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetVerticalFOV(35f, 0.3f));
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetHardLookAt(3.5f, 'X', 0f));
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetHardLookAt(3.5f, 'Y', 0f));
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetFollowOffset(0.3f, 'X', 0f));
            CameraManager.instance.StartCoroutine(CameraManager.instance.SetFollowOffset(0.3f, 'Y', 2f));
        }
    }

    public void TurnOnCharacterTab(int TabID)
    {
        UIManager.instance.characterPanel.SetActive(true); // Activate the character panel
        foreach (var panel in UIManager.instance.skinPanels)
        { 
            panel.SetActive(false); // Deactivate all skin panels
        }

        foreach (Image image in tabButtons)
        {
            image.sprite = inActiveTabBG; // Set all tab buttons to inactive background
            image.rectTransform.sizeDelta = inActiveButtonSize; // Set all tab buttons to inactive size
        }
        tabButtons[TabID].sprite = activeTabBG; // Set the selected tab button to active background
        tabButtons[TabID].rectTransform.sizeDelta = activeButtonSize; // Set the selected tab button to active size
    }



    public void TurnOnSkinTab(int TabID)
    {
        UIManager.instance.characterPanel.SetActive(false); // Activate the character panel
        foreach (var panel in UIManager.instance.skinPanels)
        {
            panel.SetActive(false); // Deactivate all skin panels
        }
        UIManager.instance.skinPanels[PlayerUltimate.instance.playerTransform.GetComponent<PlayerStat>().id].SetActive(true);

        foreach (Image image in tabButtons)
        {
            image.sprite = inActiveTabBG; // Set all tab buttons to inactive background
            image.rectTransform.sizeDelta = inActiveButtonSize; // Set all tab buttons to inactive size
        }
        tabButtons[TabID].sprite = activeTabBG; // Set the selected tab button to active background
        tabButtons[TabID].rectTransform.sizeDelta = activeButtonSize; // Set the selected tab button to active size
    }

    public void Start()
    {
        if (isTurnOnMenuAtStart)
        {
            SwitchToTabs(0); // Default to the first tab on start
        }
    }
}
