using UnityEngine;
using TMPro;

public class CleaningManager : MonoBehaviour
{
    public TextMeshProUGUI objectiveText; // UI Text to show the objective progress (e.g., "0/5 Tasks Completed")
    public GeneratorController generatorController; // Reference to the generator controller
    public CleaningTask[] cleaningTasks; // Array of all cleaning tasks

    private int totalTasks = 8; // Total number of tasks to complete (adjust this based on your actual tasks)
    private int tasksCompleted = 0; // Number of tasks completed

    void Start()
    {
        UpdateObjectiveText();
    }

    void Update()
    {
        // Check if the generator is on and update the objective text
        if (!generatorController.IsGeneratorOn)
        {
            foreach (var task in cleaningTasks)
            {
                task.CancelCleaning(); // Cancel all tasks if the generator is off
            }
        }

        // Check each cleaning task and update the completed count
        tasksCompleted = 0;
        foreach (var task in cleaningTasks)
        {
            if (task.IsCleaned)
            {
                tasksCompleted++; // Increment completed tasks count
            }
        }

        UpdateObjectiveText();
    }

    private void UpdateObjectiveText()
    {
        // Update the task progress UI
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
}
