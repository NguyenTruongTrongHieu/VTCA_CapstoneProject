using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager _instance;

    public Mission[] missions;

    public List<MissionType> missionTypes = new List<MissionType>
    {
        MissionType.KillEnemy,
        MissionType.FruitMatching,
        MissionType.UpgradeStats,
        MissionType.ReachLevel,
        MissionType.UsePowerUp
    };   // The type of the mission, e.g., KillEnemy, FruitMatching, etc.

    //public List<MissionType> missionTypes;




    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void Start()
    {
        SetUpMissionsInfo();

        //Debug.Log("How many Missions des: " + missionsDescription.missionsDescriptionTexts.Length.ToString());
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

    public void UpgradeStats()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.UpgradeStats)
            {
                missions[i].goal.currentAmount++;
                Debug.Log("Stats Upgraded: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);
                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();
                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    }

    public void ReachLevel(int level)
    {
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.ReachLevel)
            {
                if (level >= missions[i].goal.targetAmount)
                {
                    missions[i].goal.currentAmount = missions[i].goal.targetAmount; // Set current amount to target amount
                    Debug.Log("Reached Level: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);
                    if (missions[i].goal.isReached())
                    {
                        missions[i].isComplete();
                        // Update the UI or perform any other actions needed when the mission is completed
                        Debug.Log("Mission Completed: " + missions[i].description);
                    }
                }
            }
        }
    }

    public void UsePowerUp()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.UsePowerUp)
            {
                missions[i].goal.currentAmount++;
                Debug.Log("Power Up Used: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);
                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();
                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    }

    public void SetUpMissionsInfo()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            missions[i].goal = new MissionsGoal(); // Initialize the goal for each mission
            //MissionsManager._instance.missions[i].goal.targetAmount = UnityEngine.Random.Range(1, 10); // Set a random target amount for the goal
            missions[i].goal.currentAmount = 0; // Initialize current amount to 0
            missions[i].isActive = true; // Set the mission as active
            //missions[i].isCompleted = false; // Set the mission as not completed
            // Assign a random mission type from the list of available mission types
            missions[i].RandomMissionType();


            if (missions[i].isActive && !missions[i].isCompleted)
            {
                // Set the description based on the mission type
                if (missions[i].missionType == MissionType.KillEnemy)
                {
                    missions[i].goal.targetAmount = UnityEngine.Random.Range(1, 10); // Set a random target amount for the goal
                    missions[i].description = "Kill " + missions[i].goal.targetAmount + " Monsters";
                }
                else if (missions[i].missionType == MissionType.FruitMatching)
                {
                    missions[i].goal.targetAmount = UnityEngine.Random.Range(50, 100); // Set a random target amount for the goal
                    missions[i].description = "Match " + missions[i].goal.targetAmount + " Fruits";
                }
                else if (missions[i].missionType == MissionType.UpgradeStats)
                {
                    missions[i].goal.targetAmount = UnityEngine.Random.Range(5, 10); // Set a random target amount for the goal
                    missions[i].description = "Upgrade your stats " + missions[i].goal.targetAmount + " times";
                }
                else if (missions[i].missionType == MissionType.ReachLevel)
                {
                    missions[i].goal.targetAmount = UnityEngine.Random.Range(5, 10); // Set a random target amount for the goal
                    missions[i].description = "Reach Level " + missions[i].goal.targetAmount;
                }
                else if (missions[i].missionType == MissionType.UsePowerUp)
                {
                    missions[i].goal.targetAmount = UnityEngine.Random.Range(1, 10); // Set a random target amount for the goal
                    missions[i].description = "Use Power Up " + missions[i].goal.targetAmount + " times";
                }
            }
        }
    }

    //public void RandomMissionType()
    //{
    //    // Randomly select a mission type from the list of available mission types
    //    if (missionTypes.Count > 0)
    //    {
    //        int randomIndex = Random.Range(0, missionTypes.Count);
    //        missionType = missionTypes[randomIndex];
    //        missionTypes.RemoveAt(randomIndex); // Remove the selected type to avoid duplicates in the same mission

    //        //Debug.Log("Selected Mission Type: " + missionType.ToString());
    //        Debug.Log("Mission Types count " + missionTypes.Count);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No mission types available to select from.");
    //    }
    //}
}
