using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CleaningManager : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;
    public GeneratorController generatorController;
    public CleaningTask[] cleaningTasks;
    public GameObject player;

    private int totalTasks;
    private int tasksCompleted = 0;

    void Start()
    {
        totalTasks = cleaningTasks.Length;
        UpdateObjectiveText();

        // Load saved state
        LoadCleanableState();
    }

    void Update()
    {
        if (!generatorController.IsGeneratorOn)
        {
            foreach (var task in cleaningTasks)
            {
                task.CancelCleaning();
            }
        }
    }


    private void UpdateObjectiveText()
    {
        if (objectiveText != null)
        {
            objectiveText.text = $"{tasksCompleted}/{totalTasks} Tasks Completed";
        }
    }

    public void TaskCompleted()
    {
        tasksCompleted++;
        UpdateObjectiveText();
    }

    public void SaveCleanableState()
    {
        List<string> activeCleanables = new List<string>();
        foreach (var task in cleaningTasks)
        {
            if (!task.IsCleaned) activeCleanables.Add(task.name);
        }

        // Serialize the activeCleanables list
        string json = JsonUtility.ToJson(new CleanableState { cleanables = activeCleanables.ToArray() });
        PlayerPrefs.SetString("CleanableState", json);
        PlayerPrefs.SetString("LastScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        // save player location
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
        PlayerPrefs.Save();

        Debug.Log(json);
        Debug.Log("Game Saved!");
    }

    public void LoadCleanableState()
    {
        if (PlayerPrefs.HasKey("CleanableState"))
        {
            string json = PlayerPrefs.GetString("CleanableState");
            CleanableState state = JsonUtility.FromJson<CleanableState>(json);

            // Load player location
            float playerX = PlayerPrefs.GetFloat("PlayerX");
            float playerY = PlayerPrefs.GetFloat("PlayerY");
            float playerZ = PlayerPrefs.GetFloat("PlayerZ");

            player.transform.position = new Vector3(playerX, playerY, playerZ);

            tasksCompleted = 0; // Reset to calculate properly from saved state

            foreach (var task in cleaningTasks)
            {
                if (!System.Array.Exists(state.cleanables, name => name == task.name))
                {
                    task.gameObject.SetActive(false);
                    tasksCompleted++;
                }
            }

            UpdateObjectiveText(); // Ensure UI reflects the loaded state
            Debug.Log("Game Loaded!");
        }
    }
}

[System.Serializable]
public class CleanableState
{
    public string[] cleanables;
}
