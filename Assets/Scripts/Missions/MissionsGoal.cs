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

    //public void EnemyKilled()
    //{
    //    if (missionType == MissionType.KillEnemy)
    //    {
    //        currentAmount++;
    //    }
    //}

    //public void FruitMatching()
    //{
    //    if (missionType == MissionType.FruitMatching)
    //    {
    //        currentAmount++;
    //    }
    //}

    //public void UpgradeStats()
    //{
    //    if (missionType == MissionType.UpgradeStats)
    //    {
    //        currentAmount++;
    //    }
    //}

    //public void ReachLevel()
    //{
    //    if (missionType == MissionType.ReachLevel)
    //    {
    //        currentAmount++;
    //    }
    //}

    //public void UsePowerUp()
    //{
    //    if (missionType == MissionType.UsePowerUp)
    //    {
    //        currentAmount++;
    //    }
    //}
}

public enum MissionType
{
    KillEnemy,
   FruitMatching,
    UpgradeStats,
    ReachLevel,
    UsePowerUp
}
