using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int index;
    public List<GameObject> enemiesAtLevel;
    public bool isChangingScene;

    public Level(int index, List<GameObject> enemiesAtLevel)
    {
        this.index = index;
        this.enemiesAtLevel = enemiesAtLevel;
        isChangingScene = false;
    }

    public Level(int index, List<GameObject> enemiesAtLevel, bool isChangingScene) : this(index, enemiesAtLevel)
    {
        this.isChangingScene = isChangingScene;
    }

    public Level(int index)
    {
        this.index = index;
        this.enemiesAtLevel = new List<GameObject>();
        isChangingScene = false;
    }

    public Level(int index, bool isChangingScene)
    {
        this.index = index;
        this.enemiesAtLevel = new List<GameObject>();
        this.isChangingScene = isChangingScene;
    }
}
