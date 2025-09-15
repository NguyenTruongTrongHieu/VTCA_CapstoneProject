using UnityEngine;

public class OptimizeFPS : MonoBehaviour
{
    public static OptimizeFPS Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        { 
            Destroy(gameObject);
        }

        //Giới hạn fps phù hợp với thiết bị
        //Kiểm tra nếu là thiết bị yếu hơn, giảm giới hạn xuống 30 fps
        // Lấy số lượng lõi CPU và dung lượng RAM (MB)
        int processorCount = SystemInfo.processorCount;
        int systemMemoryMB = SystemInfo.systemMemorySize;

        // Giả sử: CPU yếu nếu có ít hơn 4 lõi hoặc RAM dưới 2048 MB.
        if (processorCount < 4 || systemMemoryMB < 2048)
        {
            Application.targetFrameRate = 30;
        }
        else
        {
            Application.targetFrameRate = 60;
        }

        //Tắt VSync
        QualitySettings.vSyncCount = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Giảm shadow distance khi chạy trên mobile
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            QualitySettings.shadowDistance = 10f; // Giảm khoảng cách bóng

            //Tắt soft shadow trên thiết bị yếu
            if (SystemInfo.graphicsMemorySize < 2048)
            {
                QualitySettings.shadows = ShadowQuality.HardOnly;
                //QualitySettings.shadows = ShadowQuality.Disable;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SystemInfo.batteryLevel < 0.2f)
        {
            // Nếu pin dưới 20% thì giảm FPS
            Application.targetFrameRate = 30;
        }
    }
}
