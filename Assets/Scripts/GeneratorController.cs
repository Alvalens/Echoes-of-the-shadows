using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GeneratorController : MonoBehaviour
{
    public GameObject HouseLights; // Reference to the lights group
    public Slider progressBar; // UI Slider for progress
    public TextMeshProUGUI fixText; // UI Text for the message
    public float repairTime = 10f; // Time required to repair in seconds
    public bool IsGeneratorOn => isOn;

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
    }


    void Update()
    {
        // Start repairing if player is nearby, pressing E, and the generator is off


        if (isPlayerNearby && Input.GetKey(KeyCode.E))
        {
            Debug.Log("Player is repairing the generator.");
            StartRepairing();
        }
        else if (isRepairing && (!Input.GetKey(KeyCode.E) || !isPlayerNearby))
        {
            CancelRepair(); // Stop repair if player stops pressing E or leaves the area
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
    }

    private void ToggleLights(bool state)
    {
        foreach (Light light in HouseLights.GetComponentsInChildren<Light>())
        {
            light.enabled = state;
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
            float randomTime = Random.Range(30f, 120f); // Random time in seconds
            yield return new WaitForSeconds(randomTime);

            // Turn the lights off if they're currently on
            if (isOn)
            {
                isOn = false;
                ToggleLights(false);

                // Optionally, notify the player (e.g., through UI or sound effects)
                Debug.Log("The generator has failed. The lights are off!");
            }
        }
    }
}
