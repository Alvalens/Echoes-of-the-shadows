using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CleaningTask : MonoBehaviour
{
    public string taskName;
    public TextMeshProUGUI taskPromptText;
    public GeneratorController generatorController;
    public Slider progressBar;
    public CleaningManager cleaningManager; // Reference to the CleaningManager

    private bool isPlayerNearby = false;
    private bool isCleaning = false;
    private bool isCleaned = false;
    private float cleaningTimer = 0f;
    private float cleaningDuration = 15f;

    void Start()
    {
        if (taskPromptText != null) taskPromptText.gameObject.SetActive(false);
        if (progressBar != null) progressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && !isCleaned && Input.GetKey(KeyCode.E) && generatorController.IsGeneratorOn)
        {
            StartCleaning();
        }
        else if (isCleaning && !Input.GetKey(KeyCode.E))
        {
            CancelCleaning();
        }

        if (isCleaning && Input.GetKey(KeyCode.E))
        {
            cleaningTimer += Time.deltaTime;
            if (progressBar != null)
            {
                progressBar.value = cleaningTimer / cleaningDuration;
            }

            if (cleaningTimer >= cleaningDuration)
            {
                CompleteCleaning();
            }
        }
    }

    private void StartCleaning()
    {
        if (!isCleaning)
        {
            isCleaning = true;
            cleaningTimer = 0f;

            if (taskPromptText != null)
            {
                taskPromptText.gameObject.SetActive(true);
                taskPromptText.text = "Hold E to clean...";
            }

            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.value = 0f;
            }
        }
    }

    private void CompleteCleaning()
    {
        isCleaning = false;
        isCleaned = true;

        if (taskPromptText != null)
        {
            taskPromptText.gameObject.SetActive(false);
        }

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }

        Destroy(gameObject);

        // Notify CleaningManager that this task is complete
        if (cleaningManager != null)
        {
            cleaningManager.TaskCompleted();
        }
    }

    public void CancelCleaning()
    {
        isCleaning = false;
        cleaningTimer = 0f;

        if (taskPromptText != null)
        {
            taskPromptText.gameObject.SetActive(false);
        }

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (taskPromptText != null && !isCleaned)
            {
                taskPromptText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (taskPromptText != null && !isCleaned)
            {
                taskPromptText.gameObject.SetActive(false);
            }
            CancelCleaning();
        }
    }

    public bool IsCleaned
    {
        get { return isCleaned; }
    }
}
