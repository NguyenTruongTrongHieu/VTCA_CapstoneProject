using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Level currentLevel;//Level hien tai va cac enemy cua level do, duoc tham chieu tu scene
    public Level[] levels;// Danh sach cac level trong game, cac enemy duoc tham chieu tu asset

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
