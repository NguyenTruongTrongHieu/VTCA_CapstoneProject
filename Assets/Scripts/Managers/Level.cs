using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int index;
    public List<GameObject> enemiesAtLevel;
    public string sceneName;
    public int rewardCoin;
    public bool havingBoss; // Flag to indicate if the level has a boss

    public List<Vector2Int> statesInBoard; // List to store the states in the board
    public List<Vector2Int> lockCellInBoard; // List to store the locked cells in the board

    public Level(int index, List<GameObject> enemiesAtLevel)
    {
        this.index = index;
        this.enemiesAtLevel = enemiesAtLevel;
        sceneName = "Scene1";
        statesInBoard = new List<Vector2Int>();
        lockCellInBoard = new List<Vector2Int>();
        havingBoss = false; // Default value for havingBoss
    }

    public Level(int index, List<GameObject> enemiesAtLevel, string sceneName) : this(index, enemiesAtLevel)
    {
        this.sceneName = sceneName;
        statesInBoard = new List<Vector2Int>();
        lockCellInBoard = new List<Vector2Int>();
        havingBoss = false; // Default value for havingBoss
    }

    public Level(int index)
    {
        this.index = index;
        this.enemiesAtLevel = new List<GameObject>();
        sceneName = "Scene1";
        statesInBoard = new List<Vector2Int>();
        lockCellInBoard = new List<Vector2Int>();
        havingBoss = false; // Default value for havingBoss
    }

    public Level(int index, string sceneName)
    {
        this.index = index;
        this.enemiesAtLevel = new List<GameObject>();
        this.sceneName = sceneName;
        statesInBoard = new List<Vector2Int>();
        lockCellInBoard = new List<Vector2Int>();
        havingBoss = false; // Default value for havingBoss
    }

    public Level(int index, string sceneName, bool havingBoss, int rewardCoin)
    {
        this.index = index;
        this.enemiesAtLevel = new List<GameObject>();
        this.sceneName = sceneName;
        statesInBoard = new List<Vector2Int>();
        lockCellInBoard = new List<Vector2Int>();
        this.havingBoss = havingBoss;
        this.rewardCoin = rewardCoin;
    }
}
