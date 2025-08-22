using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Mission
{
    // This class represents a mission in the game.
    public bool isActive; // Indicates if the mission is currently active
    //public string missionName;
    public string description;
    public int reward;
    public bool isCompleted;

    // List of available mission types that can be assigned to a mission


    public MissionType missionType; // The type of the mission, e.g., KillEnemy, FruitMatching, etc.

    public MissionsGoal goal; // The goal that needs to be achieved to complete the mission
    

    public void isComplete()
    {
        isActive = false; // Set the mission as inactive when completed
        isCompleted = true; // Mark the mission as completed
    }

    public void RandomMissionType()
    {
        // Randomly select a mission type from the list of available mission types
        if (MissionsManager._instance.missionTypes.Count > 0)
        {

            int randomIndex = Random.Range(0, MissionsManager._instance.missionTypes.Count);
            missionType = MissionsManager._instance.missionTypes[randomIndex];
            MissionsManager._instance.missionTypes.RemoveAt(randomIndex); // Remove the selected type to avoid duplicates in the same mission

            //Debug.Log("Selected Mission Type: " + missionType.ToString());
            Debug.Log("Mission Types count " + MissionsManager._instance.missionTypes.Count);
        }
        else
        {
            Debug.LogWarning("No mission types available to select from.");
        }
    }

    //public void CompleteMission()
    //{
    //    isCompleted = true;
    //    // Additional logic for completing the mission can be added here
    //}
}

