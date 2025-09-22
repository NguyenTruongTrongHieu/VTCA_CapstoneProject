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
        MissionType.UpgradeDamageStats,
        MissionType.UpgradeHealthStats,
        MissionType.ReachLevel,
        MissionType.UsePowerUp
    };   // The type of the mission, e.g., KillEnemy, FruitMatching, etc.

    public int missionCompletedCount; // Count of completed missions
    public bool isOpendChest;

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

        //Debug.Log("How many Missions des: " + missionsDescription.missionsDescriptionTexts.Length.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnemyKilled()
    {
        if (LevelManager.instance.currentLevel.index == 0)
            return;

        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.KillEnemy && missions[i].isCompleted == false && missions[i].isActive == true)
            {
                missions[i].goal.currentAmount++;
                Debug.Log("Enemy Killed: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);

                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();

                    //missionCompletedCount++; // Increment the count of completed missions

                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    } // DONE

    public void FruitMatching(int hasMatchedFruit)
    {
        if (LevelManager.instance.currentLevel.index == 0)
            return;

        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.FruitMatching && missions[i].isCompleted == false && missions[i].isActive == true)
            {
                missions[i].goal.currentAmount = missions[i].goal.currentAmount + hasMatchedFruit;
                Debug.Log("Fruit Matched: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);
                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();

                    //missionCompletedCount++; // Increment the count of completed missions

                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    } // DONE

    public void UpgradeDamageStats()
    {
        if (LevelManager.instance.currentLevel.index == 0)
            return;

        // Loop through all missions and check for UpgradeDamageStats type
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.UpgradeDamageStats && missions[i].isCompleted == false && missions[i].isActive == true)
            {
                // Increment the current amount for the mission goal
                missions[i].goal.currentAmount++;
                // Update the current amount for the mission goal
                Debug.Log("Damage Stats Upgraded: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);
                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();

                    //missionCompletedCount++; // Increment the count of completed missions

                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    } // DONE

    public void UpgradeHealthStats()
    {
        if (LevelManager.instance.currentLevel.index == 0)
            return;

        // Loop through all missions and check for UpgradeHealthStats type
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.UpgradeHealthStats && missions[i].isCompleted == false && missions[i].isActive == true)
            {
                // Increment the current amount for the mission goal
                missions[i].goal.currentAmount++;
                // Update the current amount for the mission goal
                Debug.Log("Health Stats Upgraded: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);

                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();

                    //missionCompletedCount++; // Increment the count of completed missions

                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    } // DONE

    public void ReachLevel(int level)
    {
        if (LevelManager.instance.currentLevel.index == 0)
            return;

        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.ReachLevel && missions[i].isCompleted == false && missions[i].isActive == true)
            {
                if (level >= missions[i].goal.targetAmount)
                {
                    missions[i].goal.currentAmount = missions[i].goal.targetAmount; // Set current amount to target amount
                    Debug.Log("Reached Level: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);
                    if (missions[i].goal.isReached())
                    {
                        missions[i].isComplete();

                        //missionCompletedCount++; // Increment the count of completed missions

                        // Update the UI or perform any other actions needed when the mission is completed
                        Debug.Log("Mission Completed: " + missions[i].description);
                    }
                }

                else
                {
                                       Debug.Log("Current Level: " + level + " is less than target level: " + missions[i].goal.targetAmount);
                }
            }
        }
    } // DONE

    public void UsePowerUp()
    {
        if (LevelManager.instance.currentLevel.index == 0)
            return;

        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].missionType == MissionType.UsePowerUp && missions[i].isCompleted == false && missions[i].isActive == true)
            {
                missions[i].goal.currentAmount++;
                Debug.Log("Power Up Used: " + missions[i].goal.currentAmount + "/" + missions[i].goal.targetAmount);
                if (missions[i].goal.isReached())
                {
                    missions[i].isComplete();

                    //missionCompletedCount++; // Increment the count of completed missions

                    // Update the UI or perform any other actions needed when the mission is completed
                    Debug.Log("Mission Completed: " + missions[i].description);
                }
            }
        }
    } // DONE

    public void SetUpMissionsInfo()
    {
        missionCompletedCount = 0; // Reset the count of completed missions at the start
        isOpendChest = false;
        SaveLoadManager.instance.isOpenedChestAtMission = isOpendChest;

        var availableLevels = LevelManager.instance.levels.Length; // Assuming there are 10 levels available
        var currentLevel = LevelManager.instance.currentLevel.index; // Get the current level number
        if (currentLevel + 1 >= availableLevels || currentLevel == 0)
        { 
            missionTypes.Remove(MissionType.ReachLevel); // Remove ReachLevel mission type if the player is at the last level
        }

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
                    missions[i].reward = 500 + Mathf.Max((LevelManager.instance.currentLevel.index - 1), 0) * 500;
                    missions[i].goal.targetAmount = 20; // Set a random target amount for the goal
                    missions[i].description = "Kill Monsters: " + missions[i].goal.targetAmount;
                    missions[i].isCompleted = false;
                    missions[i].isActive = true;
                    missions[i].isClaimed = false;
                }
                else if (missions[i].missionType == MissionType.FruitMatching)
                {
                    missions[i].reward = 300 + Mathf.Max((LevelManager.instance.currentLevel.index - 1), 0) * 500;
                    missions[i].goal.targetAmount = 150; // Set a random target amount for the goal
                    missions[i].description = "Match Fruits: " + missions[i].goal.targetAmount;
                    missions[i].isCompleted = false;
                    missions[i].isActive = true;
                    missions[i].isClaimed = false;
                }
                else if (missions[i].missionType == MissionType.UpgradeDamageStats)
                {
                    missions[i].reward = 400 + Mathf.Max((LevelManager.instance.currentLevel.index - 1), 0) * 500;
                    missions[i].goal.currentAmount = 0; // Initialize current amount to 1
                    missions[i].goal.targetAmount = 15; // Set a random target amount for the goal
                    missions[i].description = "Upgrade your Damage: " + missions[i].goal.targetAmount;
                    missions[i].isCompleted = false;
                    missions[i].isActive = true;
                    missions[i].isClaimed = false;
                }
                else if (missions[i].missionType == MissionType.UpgradeHealthStats)
                {
                    missions[i].reward = 400 + Mathf.Max((LevelManager.instance.currentLevel.index - 1), 0) * 500;
                    missions[i].goal.currentAmount = 0; // Initialize current amount to 1
                    missions[i].goal.targetAmount = 15; // Set a random target amount for the goal
                    missions[i].description = "Upgrade your Health: " + missions[i].goal.targetAmount;
                    missions[i].isCompleted = false;
                    missions[i].isActive = true;
                    missions[i].isClaimed = false;
                }
                else if (missions[i].missionType == MissionType.ReachLevel)
                {
                    // Set the target amount based on the current level and available levels
                    missions[i].goal.currentAmount = currentLevel; // Initialize current amount to the current level

                    missions[i].reward = 250 + Mathf.Max((LevelManager.instance.currentLevel.index - 1), 0) * 500;
                    missions[i].goal.targetAmount = missions[i].goal.currentAmount + 1; // Set a random target amount for the goal
                    missions[i].description = "Reach Level: " + missions[i].goal.targetAmount;
                    missions[i].isCompleted = false;
                    missions[i].isActive = true;
                    missions[i].isClaimed = false;



                    //if ((availableLevels - currentLevel) == 3)
                    //{
                    //    missions[i].reward = 300;
                    //    missions[i].goal.targetAmount = 3; // Set a random target amount for the goal
                    //    missions[i].description = "Reach Level " + missions[i].goal.targetAmount;
                    //}
                    //else if ((availableLevels - currentLevel) == 2)
                    //{
                    //    missions[i].reward = 250;
                    //    missions[i].goal.targetAmount = 2; // Set a random target amount for the goal
                    //    missions[i].description = "Reach Level " + missions[i].goal.targetAmount;
                    //}
                    //else if ((availableLevels - currentLevel) == 1)
                    //{
                    //    missions[i].reward = 250;
                    //    Debug.Log("Set goal to 1 level with aditional condition");
                    //}
                }
                else if (missions[i].missionType == MissionType.UsePowerUp)
                {
                    missions[i].reward = 300 + Mathf.Max((LevelManager.instance.currentLevel.index - 1), 0) * 500;
                    missions[i].goal.targetAmount = 10; // Set a random target amount for the goal
                    missions[i].description = "Use Ultimate: " + missions[i].goal.targetAmount;
                    missions[i].isCompleted = false;
                    missions[i].isActive = true;
                    missions[i].isClaimed = false;
                }
            }
        }
    }

    public bool ChestOpening()
    {
        bool result = false;

        Debug.Log("Chest Opening called. Current completed missions: " + missionCompletedCount);

        if (isOpendChest)
        { 
            result = false;
            return result;
        }    


        if (missionCompletedCount == 3)
        {
            result = true;

        }
        else if (missionCompletedCount != 3)
        {
            result = false;
            return result;
        }

        // Additional check to ensure all missions are claimed
        //foreach (var mission in missions)
        //{ 
        //    if(mission.isClaimed == false)
        //     {
        //        result = false;
        //        return result;
        //    }
        //}

        result = true;
        return result;
    } 

    public void RewardClaiming(int missionIndex)
    {

        CurrencyManager.instance.StartCoroutine(CurrencyManager.instance.AddCoins(UIManager.instance.missionsDescriptionTexts[missionIndex].transform.position, missions[missionIndex].reward, false, 0f)); // Add coins to the player's currency
        missions[missionIndex].isClaimed = true; // Mark the mission as claimed
        missionCompletedCount++;
        AudioManager.instance.PlaySFX("MissionClaim");
        UIManager.instance.UpdateMissionCompletedProgress();

        //for (int i = 0; i < missions.Length; i++)
        //{
        //    if (missions[i].isCompleted && !missions[i].isActive && !missions[i].isClaimed)
        //    {
        //        // Here you can implement the logic to give rewards to the player
        //        // For example, you can give coins, gems, or any other rewards based on the mission type
        //        Debug.Log("Reward Claimed for Mission: " + missions[i].description);

        //    }
        //}
    }

    public void TestChestOpening()
    {
        // This method is for testing purposes only, to simulate chest opening
        missionCompletedCount++;
    }

    public bool CheckIsMissionsCompleted()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].isCompleted && !missions[i].isActive)
            {
                return true;
            }
        }

        return false;
    }

    public void ResetMission()
    {
        missionTypes = new List<MissionType>
        {
            MissionType.KillEnemy,
            MissionType.FruitMatching,
            MissionType.UpgradeDamageStats,
            MissionType.UpgradeHealthStats,
            MissionType.ReachLevel,
            MissionType.UsePowerUp
        };

        for (int i = 0; i < missions.Length; i++)
        {
            missions[i].isActive = false;
            missions[i].isCompleted = false;
            missions[i].isClaimed = false;
            missions[i].goal.currentAmount = 0;
            missions[i].goal.targetAmount = 0;
            missions[i].description = "";
            missions[i].reward = 0;
        }
        missionCompletedCount = 0; // Reset the count of completed missions
        SetUpMissionsInfo();
    }

}
