using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager _instance;

    public Text[] missionsDescription;
    public Mission[] missions;
    public Text[] rewardAmount;



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

    public void SetMissionDescription()
    {
        for(int i = 0; i < missionsDescription.Length; i++)
        {
            //int randomIndex = Random.Range(5, 10);
            //missions[i].KillMissionCreated(randomIndex, 25);


            if (missions[i].isActive && !missions[i].isCompleted)
            {
                missionsDescription[i].text = missions[i].description;
                //rewardAmount[i].text = "Reward: " + missions[i].reward.ToString();
            }
            else if (missions[i].isCompleted)
            {
                missionsDescription[i].text = "Mission Completed: ";
            }
            else
            {
                missionsDescription[i].text = "No active mission";
            }
        }
    }
}
