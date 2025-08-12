using UnityEngine;


[System.Serializable]
public class Mission
{
    // This class represents a mission in the game.
    public bool isActive; // Indicates if the mission is currently active
    public string missionName;
    public string description;
    public int reward;
    public bool isCompleted;
    public Mission(string name, string desc, int rewardAmount)
    {
        missionName = name;
        description = desc;
        reward = rewardAmount;
        isCompleted = false;
    }
    public void CompleteMission()
    {
        isCompleted = true;
        // Additional logic for completing the mission can be added here
    }
}

