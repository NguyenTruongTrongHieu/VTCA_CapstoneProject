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

    public MissionType missionType; // The type of the mission, e.g., KillEnemy, FruitMatching, etc.
    public MissionsGoal goal; // The goal that needs to be achieved to complete the mission
    

    public void isComplete()
    {
        isActive = false; // Set the mission as inactive when completed
        isCompleted = true; // Mark the mission as completed
    }


    public Mission( )
    {
        
    }

    public void CreateMission(string description, int reward)
    {
        this.description = description;
        this.reward = reward;
        isActive = true; // Set the mission as active when created
        isCompleted = false; // Initially, the mission is not completed
    }

    public void KillMissionCreated(int targetAmount, int reward)
    {
        CreateMission("Kill " + targetAmount + " Monster", reward);
    }

    //public void CompleteMission()
    //{
    //    isCompleted = true;
    //    // Additional logic for completing the mission can be added here
    //}
}

