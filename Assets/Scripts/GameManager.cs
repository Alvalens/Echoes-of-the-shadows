using UnityEngine;
using TMPro; // If using TextMeshPro
using UnityEngine.SceneManagement; // For scene management

public class Gamemanager : MonoBehaviour
{
    public float realTimeDuration = 600f; // 10 minutes in seconds
    private float timer;
    public TextMeshProUGUI timerText; // Assign the TimerText in the Inspector

    private int startHour = 22; // 10:00 PM in 24-hour format
    private int endHour = 3; // 3:00 AM in 24-hour format

    public CleaningManager cleaningManager; // Reference to CleaningManager
    private bool isGameWon = false; // To avoid triggering win multiple times

    void Start()
    {
        timer = realTimeDuration;
        UpdateClockDisplay();
    }

    void Update()
    {
        if (!isGameWon)
        {
            // Countdown timer
            if (timer > 0)
            {
                timer -= Time.deltaTime; // Reduce time by the delta time
                UpdateClockDisplay();

                // Check win condition
                CheckWinCondition();

                // If timer hits 0, switch to Game Over scene
                if (timer <= 0)
                {
                    GameOver();
                }
            }
        }
    }

    void UpdateClockDisplay()
    {
        // Calculate the current time based on the timer
        float elapsedRatio = 1 - (timer / realTimeDuration); // Percentage of time elapsed
        int totalGameMinutes = (5 * 60); // 5 hours in minutes (10:00 PM to 3:00 AM)
        int currentMinutes = Mathf.FloorToInt(elapsedRatio * totalGameMinutes);

        // Convert elapsed game minutes to hours and minutes
        int currentHour = (startHour + (currentMinutes / 60)) % 24; // Handle rollover past midnight
        int currentMinute = currentMinutes % 60;

        // Display time in 12-hour format with AM/PM
        string period = currentHour >= 12 ? "PM" : "AM";
        int displayHour = currentHour > 12 ? currentHour - 12 : (currentHour == 0 ? 12 : currentHour);
        timerText.text = $"{displayHour:00}:{currentMinute:00} {period}";
    }

    void CheckWinCondition()
    {
        if (cleaningManager != null && cleaningManager.tasksCompleted == cleaningManager.cleaningTasks.Length)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        isGameWon = true; // Mark the game as won
        Debug.Log("You won!");

        // Save the final time elapsed in PlayerPrefs
        float timeElapsed = realTimeDuration - timer;
        PlayerPrefs.SetFloat("FinalTime", timeElapsed);
        PlayerPrefs.Save();

        // Load the Wining scene
        SceneManager.LoadScene("Wining");
    }


    void GameOver()
    {
        SceneManager.LoadScene("Game Over");
    }
}