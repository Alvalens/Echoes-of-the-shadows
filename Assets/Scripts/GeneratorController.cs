using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GeneratorController : MonoBehaviour
{
    public GameObject HouseLights; // Reference to the lights group
    public Slider progressBar; // UI Slider for progress
    public TextMeshProUGUI fixText; // UI Text for the message
    public float repairTime = 20f; // Time required to repair in seconds
    public bool IsGeneratorOn => isOn;
    public TextMeshProUGUI taskPromptText;
    public GhostController ghostController;

    public AudioSource generatorAudio; // AudioSource reference for the generator sound
    public AudioSource fixingAudio; // AudioSource reference for the fixing sound
    public AudioSource electricityAudio; // AudioSource reference for the electricity sound

    private bool isPlayerNearby = false;
    private bool isRepairing = false;
    private float repairProgress = 0f;
    private bool isOn = true;

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.value = 0f; // Initialize slider value
            progressBar.gameObject.SetActive(false); // Hide progress bar initially
        }

        if (fixText != null)
        {
            fixText.gameObject.SetActive(false); // Hide the text initially
        }

        // Start the coroutine for random light turn-off
        StartCoroutine(RandomlyTurnOffLights());

        // Start with generator audio playing if it's on
        if (generatorAudio != null && isOn)
        {
            generatorAudio.Play(); // Play audio if the generator is on
        }
    }

    void Update()
    {
        // Start repairing if player is nearby, pressing E, and the generator is off
        if (isPlayerNearby && Input.GetKey(KeyCode.E) && !isOn)
        {
            StartRepairing();
        }
        else if (isRepairing && (!Input.GetKey(KeyCode.E) || !isPlayerNearby))
        {
            CancelRepair(); // Stop repair if player stops pressing E or leaves the area
        }
    }

    private void PromptFixGenerator()
    {
        if (taskPromptText != null)
        {
            taskPromptText.text = "I should fix the generator first!";
            taskPromptText.gameObject.SetActive(true);
        }
    }

    private void StartRepairing()
    {
        if (!isRepairing)
        {
            isRepairing = true;
        }

        if (progressBar != null && !progressBar.gameObject.activeSelf)
        {
            progressBar.gameObject.SetActive(true); // Always ensure progress bar is visible
        }

        // Start the fixing audio when repairing starts
        if (fixingAudio != null && !fixingAudio.isPlaying)
        {
            fixingAudio.Play(); // Play fixing sound
        }

        // Increment progress
        repairProgress += Time.deltaTime / repairTime;
        if (progressBar != null)
        {
            progressBar.value = repairProgress;

            // Force a UI update (helpful for debugging UI redraw issues)
            Canvas.ForceUpdateCanvases();
        }

        // Check if repair is complete
        if (repairProgress >= 1f)
        {
            CompleteRepair();
        }
    }

    private void CancelRepair()
    {
        isRepairing = false;
        repairProgress = 0f;

        if (progressBar != null)
        {
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false); // Hide the progress bar
        }

        // Stop the fixing audio if the repair is cancelled
        if (fixingAudio != null && fixingAudio.isPlaying)
        {
            fixingAudio.Stop();
        }
    }

    private void CompleteRepair()
    {
        isRepairing = false;
        repairProgress = 0f;

        if (progressBar != null)
        {
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false); // Hide the progress bar
        }

        isOn = true; // Set generator state back to "on"
        ToggleLights(true); // Turn on lights when repair is complete

        // Stop the fixing audio and start the generator audio
        if (fixingAudio != null && fixingAudio.isPlaying)
        {
            fixingAudio.Stop(); // Stop fixing sound
        }

        // Start the generator audio when it's back on
        if (generatorAudio != null && !generatorAudio.isPlaying)
        {
            generatorAudio.Play(); // Play generator sound
        }

        // Stop ghost spawning when the generator is repaired
        ghostController.DeactivateGhost();

        // Clean up UI taskPromptText
        if (taskPromptText != null)
        {
            taskPromptText.gameObject.SetActive(false);
        }
    }

    private void ToggleLights(bool state)
    {
        foreach (Light light in HouseLights.GetComponentsInChildren<Light>())
        {
            light.enabled = state;
        }

        // Play electricity sound when toggling lights
        if (electricityAudio != null)
        {
            electricityAudio.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // Player is near the generator

            if (fixText != null)
            {
                fixText.gameObject.SetActive(true); // Show the text when the player is near
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player leaves the generator area

            if (fixText != null)
            {
                fixText.gameObject.SetActive(false); // Hide the text when the player leaves
            }

            CancelRepair(); // Stop the repair process if the player leaves
        }
    }

    private IEnumerator RandomlyTurnOffLights()
    {
        while (true)
        {
            // Wait for a random time between 1 and 4 minutes
            float randomTime = Random.Range(20f, 60f); // Random time in seconds
            yield return new WaitForSeconds(randomTime);

            // Turn the lights off if they're currently on
            if (isOn)
            {
                isOn = false;
                ToggleLights(false);

                // Stop the generator audio when it's turned off
                if (generatorAudio != null && generatorAudio.isPlaying)
                {
                    generatorAudio.Stop();
                }

                // Trigger ghost spawning when the generator fails
                ghostController.ActivateGhost();

                // Notify the player (e.g., through UI or sound effects)
                Debug.Log("The generator has failed. The lights are off!");
                PromptFixGenerator();
            }
        }
    }
}
