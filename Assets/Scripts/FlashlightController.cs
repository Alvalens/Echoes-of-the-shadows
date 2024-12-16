using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;                  // Reference to the Spotlight component
    public float batteryLife = 150f;          // 3 minutes of battery life
    public bool isOn = true;                  // Flashlight state
    public Collider detectionCollider;        // Trigger collider to detect ghosts
    public TextMeshProUGUI batteryText;       // UI Text to show battery life
    public GhostController ghostController;   // Reference to the GhostController script
    public AudioSource flashlightAudio;       // AudioSource for flashlight sound

    void Start()
    {
        // Get the Light and Collider components if not already assigned
        if (!flashlight) flashlight = GetComponent<Light>();
        if (!detectionCollider) detectionCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // Toggle flashlight with F key
        if (Input.GetButtonDown("Toggle"))
        {
            ToggleFlashlight();
        }

        // Drain battery if the flashlight is on
        if (isOn && batteryLife > 0)
        {
            batteryLife -= Time.deltaTime;

            // Turn off flashlight when battery is depleted
            if (batteryLife <= 0)
            {
                batteryLife = 0;
                TurnOffFlashlight();
            }
        }

        // Update battery text
        UpdateBatteryText();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check for collision with a ghost when the flashlight is on
        if (isOn && other.CompareTag("ghost"))
        {
            Debug.Log("Ghost detected! Teleporting...");
            ghostController.TeleportToRandomSpawn(); // Teleport the ghost
        }
    }

    void ToggleFlashlight()
    {
        if (batteryLife <= 0)
        {
            return;
        }
        isOn = !isOn;
        flashlight.enabled = isOn;
        detectionCollider.enabled = isOn; // Enable/Disable ghost detection

        // Play audio for toggling flashlight
        if (flashlightAudio != null)
        {
            flashlightAudio.time = 0.08f; // Set audio start time to 0.15 seconds
            flashlightAudio.Play();
        }
    }

    void TurnOffFlashlight()
    {
        isOn = false;
        flashlight.enabled = false;
        detectionCollider.enabled = false;
    }

    void UpdateBatteryText()
    {
        // Calculate battery percentage
        float batteryPercentage = (batteryLife / 150f) * 100f;

        // Update the text component
        batteryText.text = $"{Mathf.CeilToInt(batteryPercentage)}%";
    }
}
