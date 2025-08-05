using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager _instance;

    public Text[] missionsDescription;



    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMissionDescription(int missionIndex, string description)
    {
        if (missionIndex < 0 || missionIndex >= missionsDescription.Length)
        {
            Debug.LogError("Mission index out of bounds");
            return;
        }
        missionsDescription[missionIndex].text = description;
    }
}
