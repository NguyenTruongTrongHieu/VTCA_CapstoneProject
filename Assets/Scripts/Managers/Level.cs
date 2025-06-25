using UnityEngine;

[System.Serializable]
public class Level
{
    public int index;
    public GameObject[] enemiesAtLevel;
    public bool isChangingScene;

    public Level(int index, GameObject[] enemiesAtLevel)
    {
        this.index = index;
        this.enemiesAtLevel = enemiesAtLevel;
        isChangingScene = false;
    }

    public Level(int index, GameObject[] enemiesAtLevel, bool isChangingScene) : this(index, enemiesAtLevel)
    {
        this.isChangingScene = isChangingScene;
    }

    public Level(int index)
    {
        this.index = index;
        this.enemiesAtLevel = new GameObject[4];
        isChangingScene = false;
    }

    public Level(int index, bool isChangingScene)
    {
        this.index = index;
        this.enemiesAtLevel = new GameObject[4];
        this.isChangingScene = isChangingScene;
    }
}
