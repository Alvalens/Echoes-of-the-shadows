using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BedInteraction : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI promptText;
    public float saveDuration = 5f;

    private bool isPlayerNearby = false;
    private bool isSaving = false;
    private float saveTimer = 0f;
    private CleaningManager cleaningManager;

    void Start()
    {
        cleaningManager = FindObjectOfType<CleaningManager>();
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetButton("Interact"))
        {
            if (!isSaving)
            {
                isSaving = true;
                saveTimer = 0f;
                if (progressBar != null) progressBar.gameObject.SetActive(true);
            }

            saveTimer += Time.deltaTime;
            if (progressBar != null)
            {
                progressBar.value = saveTimer / saveDuration;
            }

            if (saveTimer >= saveDuration)
            {
                SaveGame();
            }
        }
        else if (isSaving && !Input.GetButton("Interact"))
        {
            CancelSaving();
        }
    }

    private void SaveGame()
    {
        isSaving = false;
        saveTimer = 0f;

        if (progressBar != null) progressBar.gameObject.SetActive(false);

        cleaningManager.SaveCleanableState();

        if (promptText != null)
        {
            promptText.text = "Game Saved!";
            Invoke(nameof(HidePrompt), 2f);
        }
    }

    private void CancelSaving()
    {
        isSaving = false;
        saveTimer = 0f;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0f;
        }
    }

    private void HidePrompt()
    {
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "Hold E to save game";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            CancelSaving();
            if (promptText != null) promptText.gameObject.SetActive(false);
        }
    }
}
