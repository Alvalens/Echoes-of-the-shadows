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
    private float remainingTimeUntilFailure; // Time left before the next blackout
    private bool isTimerActive = true; // Flag to control the timer


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

        remainingTimeUntilFailure = Random.Range(20f, 60f); // Set initial random time
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
        if (isPlayerNearby && Input.GetButton("Interact") && !isOn)
        {
            StartRepairing();
        }
        else if (isRepairing && (!Input.GetButton("Interact") || !isPlayerNearby))
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
            progressBar.gameObject.SetActive(false);
        }

        isOn = true; // Turn generator back on
        ToggleLights(true);

        // Resume the timer
        isTimerActive = true;

        if (fixingAudio != null && fixingAudio.isPlaying)
        {
            fixingAudio.Stop();
        }

        if (generatorAudio != null && !generatorAudio.isPlaying)
        {
            generatorAudio.Play();
        }

        ghostController.DeactivateGhost();

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

        if (electricityAudio != null)
        {
            electricityAudio.Play();
        }

        // Pause the blackout timer if lights are off
        isTimerActive = state;
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
            isPlayerNearby = false;

            if (fixText != null)
            {
                fixText.gameObject.SetActive(false);
            }

            CancelRepair();
        }
    }

    private IEnumerator RandomlyTurnOffLights()
    {
        while (true)
        {
            while (!isTimerActive) yield return null; // Wait if the timer is paused

            // Countdown for the remaining time
            yield return new WaitForSeconds(remainingTimeUntilFailure);

            // Turn off the generator and lights if they are on
            if (isOn)
            {
                isOn = false;
                ToggleLights(false);
                ghostController.ActivateGhost();

                // Stop the generator audio
                if (generatorAudio != null && generatorAudio.isPlaying)
                {
                    generatorAudio.Stop();
                }

                // Notify the player
                Debug.Log("The generator has failed. The lights are off!");
                PromptFixGenerator();
            }

            // Generate a new random time for the next blackout
            remainingTimeUntilFailure = Random.Range(20f, 60f);
        }
    }
}
