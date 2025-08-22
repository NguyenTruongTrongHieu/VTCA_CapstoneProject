using UnityEngine;

public class MissionsGoal
{
    public MissionType missionType;

    public int targetAmount;
    public int currentAmount;

    public bool isReached()
    {
        return (currentAmount >= targetAmount);
    }
}

public enum MissionType
{
    KillEnemy,
   FruitMatching,
    UpgradeStats,
    ReachLevel,
    UsePowerUp
}
