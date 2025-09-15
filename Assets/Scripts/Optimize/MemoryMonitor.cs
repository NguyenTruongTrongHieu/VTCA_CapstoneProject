using UnityEngine;

public class MemoryMonitor : MonoBehaviour
{
    private float lastMemoryUsage;
    private int memoryIncreaseCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("CheckMemoryUsage", 30f, 30f);
        //lastMemoryUsage = UnityEngine.Profiling.
        //    Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f); // Convert to MB
    }

    void CheckMemoryUsage()
    {
        float currentMemoryUsage = UnityEngine.Profiling.
            Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f); // Convert to MB

        if (currentMemoryUsage > lastMemoryUsage * 1.2f)
        {
            // Memory usage increased by more than 20%
            memoryIncreaseCount++;
            if (memoryIncreaseCount >= 3)
            {
                //increased memory usage 3 times
                //Start cleaning
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                Debug.LogWarning("Memory usage increased significantly. " +
                    "Unloading unused assets and collecting garbage.");
                memoryIncreaseCount = 0; // Reset the count after cleaning
            }

            ////Start cleaning
            //Resources.UnloadUnusedAssets();
            //System.GC.Collect();
            //Debug.LogWarning("Memory usage increased significantly. " +
            //    "Unloading unused assets and collecting garbage.");
        }
        else
        {
            memoryIncreaseCount = 0;
        }

        lastMemoryUsage = currentMemoryUsage;
    }
}
