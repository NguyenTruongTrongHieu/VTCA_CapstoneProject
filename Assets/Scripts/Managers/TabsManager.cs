using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    public static TabsManager instance; // Singleton instance

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
    }

    public void Start()
    {
        SwitchToTabs(0); // Default to the first tab on start
    }
}
