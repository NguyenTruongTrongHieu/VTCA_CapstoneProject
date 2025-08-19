using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager _instance;

    public UIManager missionsDescription;
    public Mission[] missions;
    public Text[] rewardAmount;
    public List<MissionType> missionTypes;




    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void Start()
    {
        //Debug.Log("How many Missions des: " + missionsDescription.missionsDescriptionTexts.Length.ToString());

        missionTypes = new List<MissionType>
                {
                 MissionType.KillEnemy,
                 MissionType.FruitMatching,
                 MissionType.UpgradeStats,
                 MissionType.ReachLevel,
                 MissionType.UsePowerUp
                };
    }

// Update is called once per frame
void Update()
    {
        
    }

    public void EnemyKilled()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.KillEnemy)
            {
               missions[i].goal.currentAmount++;
                Debug.Log("Enemy Killed: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);

                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();
                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    }

    public void FruitMatching(int hasMatchedFruit)
    {
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.FruitMatching)
            {
                missions[i].goal.currentAmount = missions[i].goal.currentAmount + hasMatchedFruit;
                Debug.Log("Fruit Matched: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);
                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();
                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    }

    //public void SetMissionDescription()
    //{
    //    for(int i = 0; i < UIManager.instance.missionsDescriptionTexts.Length; i++)
    //    {
    //        missions[i].goal = new MissionsGoal(); // Initialize the goal for each mission
    //        missions[i].goal.targetAmount = Random.Range(1, 10); // Set a random target amount for the goal
    //        missions[i].goal.currentAmount = 0; // Initialize current amount to 0
    //        missions[i].isActive = true; // Set the mission as active
    //        missions[i].RandomMissionType();


    //        if (missions[i].isActive && !missions[i].isCompleted)
    //        {
    //            // Set the description based on the mission type
    //            if (missions[i].missionType == MissionType.KillEnemy)
    //            {
    //                missions[i].description = "Kill " + missions[i].goal.targetAmount + " Monsters";
    //                UIManager.instance.missionsDescriptionTexts[i].text = missions[i].description;
    //            }
    //            else if (missions[i].missionType == MissionType.FruitMatching)
    //            {
    //                missions[i].description = "Match " + missions[i].goal.targetAmount + " Fruits";
    //                UIManager.instance.missionsDescriptionTexts[i].text = missions[i].description;
    //            }
    //            else if (missions[i].missionType == MissionType.UpgradeStats)
    //            {
    //                missions[i].description = "Upgrade your stats " + missions[i].goal.targetAmount + " times";
    //                UIManager.instance.missionsDescriptionTexts[i].text = missions[i].description;
    //            }
    //            else if (missions[i].missionType == MissionType.ReachLevel)
    //            {
    //                missions[i].description = "Reach Level " + missions[i].goal.targetAmount;
    //                UIManager.instance.missionsDescriptionTexts[i].text = missions[i].description;
    //            }
    //            else if (missions[i].missionType == MissionType.UsePowerUp)
    //            {
    //                missions[i].description = "Use Power Up " + missions[i].goal.targetAmount + " times";
    //                UIManager.instance.missionsDescriptionTexts[i].text = missions[i].description;
    //            } 
    //            //rewardAmount[i].text = "Reward: " + missions[i].reward.ToString();
    //        }
    //        else if (missions[i].isCompleted)
    //        {
    //            UIManager.instance.missionsDescriptionTexts[i].text = "Mission Completed: ";
    //        }
    //        else
    //        {
    //            UIManager.instance.missionsDescriptionTexts[i].text = "No active mission";
    //        }
    //    }
    //}
}
