using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BedInteraction : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI promptText;
    public AudioSource savingAudio;   // Audio untuk suara proses menyimpan
    public AudioSource savedAudio;    // Audio untuk suara selesai menyimpan
    public float saveDuration = 5f;   // Durasi untuk proses menyimpan

    private bool isPlayerNearby = false;
    private bool isSaving = false;
    private bool hasSaved = false;  // Flag untuk mencegah penyimpanan berulang
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
        if (isPlayerNearby && Input.GetKey(KeyCode.E) && !hasSaved)  // Tambahkan pengecekan hasSaved
        {
            if (!isSaving)
            {
                isSaving = true;
                saveTimer = 0f;
                if (progressBar != null) progressBar.gameObject.SetActive(true);

                // Play the saving sound only once when saving starts
                if (savingAudio != null && !savingAudio.isPlaying)
                {
                    savingAudio.Play();
                }
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
        else if (isSaving && !Input.GetKey(KeyCode.E))
        {
            CancelSaving();
        }
    }

    private void SaveGame()
    {
        isSaving = false;
        saveTimer = 0f;
        hasSaved = true;  // Menandai bahwa game telah disimpan

        if (progressBar != null) progressBar.gameObject.SetActive(false);

        cleaningManager.SaveCleanableState();

        if (promptText != null)
        {
            promptText.text = "Game Saved!";
            Invoke(nameof(HidePrompt), 2f);
        }

        // Stop the saving sound and play the saved sound when saving is complete
        if (savingAudio != null && savingAudio.isPlaying)
        {
            savingAudio.Stop();  // Stop the saving sound
        }

        // Play the saved sound but cut the first 0.2 seconds
        if (savedAudio != null && !savedAudio.isPlaying)
        {
            savedAudio.time = 0.5f;
            savedAudio.Play();       // Putar audio mulai dari waktu yang sudah dipotong
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

        // Optionally stop the saving sound if saving is canceled
        if (savingAudio != null && savingAudio.isPlaying)
        {
            savingAudio.Stop();  // Stop the saving sound if the action is canceled
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

            // Reset hasSaved flag when player exits the zone
            hasSaved = false;
        }
    }
}
