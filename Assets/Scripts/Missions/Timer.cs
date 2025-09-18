using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }
    [SerializeField] private float remainingTime = 36000f; // Default starting time


    private void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this; // Set this as the Singleton instance
        }
    }
     void Start()
    {
      
    }

     void Update()
    {

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            //Debug.Log(GetFormattedTime());
        }
        else
        {
            remainingTime = 0; // Ensure the time doesn't go negative
            //timerText.text = "00:00";

            Debug.Log("Time's up!");
        }
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            TimeSetup();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            TimeSetup();
        }
    }

    public string GetFormattedTime()
    {
        // Tính tổng số giây đã làm tròn
        int totalSeconds = Mathf.FloorToInt(remainingTime);

        // Tính giờ, phút, giây từ tổng số giây
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        // Định dạng chuỗi theo đúng cú pháp
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public void ResetMissionTime()
    {
        if (remainingTime > 0)
        remainingTime = 1f; // Reset to default time

        else if (remainingTime < 0)
                        remainingTime = 0f; // Reset to default time

        else
            remainingTime = 36000f; // Reset to default time
    }

    public void TimeSetup()
    {
        // Check if a saved time value exists
        if (PlayerPrefs.HasKey("SavedTime"))
        {
            remainingTime = PlayerPrefs.GetFloat("SavedTime");

            // Check for a saved timestamp
            if (PlayerPrefs.HasKey("ExitTimestamp"))
            {
                string exitTimestampString = PlayerPrefs.GetString("ExitTimestamp");
                long exitTimeTicks = long.Parse(exitTimestampString);

                // Reconstruct the DateTime object from ticks
                System.DateTime exitTime = new System.DateTime(exitTimeTicks);
                System.DateTime currentTime = System.DateTime.UtcNow;

                // Calculate the time difference
                System.TimeSpan timeDifference = currentTime.Subtract(exitTime);

                // Subtract the elapsed time from the saved remaining time
                remainingTime -= (float)timeDifference.TotalSeconds;

                //Debug.Log("Time elapsed while away: " + timeDifference.TotalSeconds.ToString("F2") + " seconds.");
                //Debug.Log("New remaining time after accounting for offline time: " + remainingTime.ToString("F2"));
            }
        }
    }

    public void SaveData()
    {
        // Save the current remaining time
        PlayerPrefs.SetFloat("SavedTime", remainingTime);

        // Save the timestamp of when the game was closed
        long exitTimeTicks = System.DateTime.UtcNow.Ticks;
        PlayerPrefs.SetString("ExitTimestamp", exitTimeTicks.ToString());

        PlayerPrefs.Save();
        //Debug.Log("Saved exit timestamp: " + exitTimeTicks);
    }
}