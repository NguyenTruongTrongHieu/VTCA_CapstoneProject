using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }
    [SerializeField] private float remainingTime = 5f; // Default starting time


    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // Destroy this new instance
        }
        else
        {
            Instance = this; // Set this as the Singleton instance
            DontDestroyOnLoad(this.gameObject); // Optional: Keep it alive between scenes
        }
    }
    public void Start()
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

                Debug.Log("Time elapsed while away: " + timeDifference.TotalSeconds.ToString("F2") + " seconds.");
                Debug.Log("New remaining time after accounting for offline time: " + remainingTime.ToString("F2"));
            }
        }
    }

    public void Update()
    {

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            // Calculate minutes and seconds
            //int minutes = Mathf.FloorToInt(remainingTime / 60);
            //int seconds = Mathf.FloorToInt(remainingTime % 60);

            Debug.Log(GetFormattedTime());

            // Format the string to display minutes and seconds
            // "D2" ensures that single-digit numbers are padded with a leading zero (e.g., 9 becomes 09)
            //timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }
        else
        {
            remainingTime = 0; // Ensure the time doesn't go negative
            //timerText.text = "00:00";

            Debug.Log("Time's up!");
        }
    }

    public void OnApplicationQuit()
    {
        // Save the current remaining time
        PlayerPrefs.SetFloat("SavedTime", remainingTime);

        // Save the timestamp of when the game was closed
        long exitTimeTicks = System.DateTime.UtcNow.Ticks;
        PlayerPrefs.SetString("ExitTimestamp", exitTimeTicks.ToString());

        PlayerPrefs.Save();
        Debug.Log("Saved exit timestamp: " + exitTimeTicks);
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        return string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public void Reset()
    {
        remainingTime = 5f; // Reset to default time
    }
}