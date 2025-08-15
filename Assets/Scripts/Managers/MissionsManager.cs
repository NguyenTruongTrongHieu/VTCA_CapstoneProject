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
        SetMissionDescription();


    }

// Update is called once per frame
void Update()
    {
        
    }

    public void SetMissionDescription()
    {
        for(int i = 0; i < missionsDescription.Length; i++)
        {
            missions[i].goal = new MissionsGoal(); // Initialize the goal for each mission
            missions[i].goal.targetAmount = Random.Range(1, 10); // Set a random target amount for the goal
            missions[i].goal.currentAmount = 0; // Initialize current amount to 0
            missions[i].isActive = true; // Set the mission as active
            missions[i].RandomMissionType();


            if (missions[i].isActive && !missions[i].isCompleted)
            {
                // Set the description based on the mission type
                if (missions[i].missionType == MissionType.KillEnemy)
                {
                    missions[i].description = "Kill " + missions[i].goal.targetAmount + " Monsters";
                    missionsDescription[i].text = missions[i].description;
                }
                else if (missions[i].missionType == MissionType.FruitMatching)
                {
                    missions[i].description = "Match " + missions[i].goal.targetAmount + " Fruits";
                    missionsDescription[i].text = missions[i].description;
                }
                else if (missions[i].missionType == MissionType.UpgradeStats)
                {
                    missions[i].description = "Upgrade your stats " + missions[i].goal.targetAmount + " times";
                    missionsDescription[i].text = missions[i].description;
                }
                else if (missions[i].missionType == MissionType.ReachLevel)
                {
                    missions[i].description = "Reach Level " + missions[i].goal.targetAmount;
                    missionsDescription[i].text = missions[i].description;
                }
                else if (missions[i].missionType == MissionType.UsePowerUp)
                {
                    missions[i].description = "Use Power Up " + missions[i].goal.targetAmount + " times";
                    missionsDescription[i].text = missions[i].description;
                } 
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
