using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;                  // Reference to the Spotlight component
    public float batteryLife = 180f;          // 3 minutes of battery life
    public bool isOn = true;                  // Flashlight state
    public Collider detectionCollider;        // Trigger collider to detect ghosts
    public TextMeshProUGUI batteryText;       // UI Text to show battery life
    public GhostController ghostController;   // Reference to the GhostController script

    void Start()
    {
        // Get the Light and Collider components if not already assigned
        if (!flashlight) flashlight = GetComponent<Light>();
        if (!detectionCollider) detectionCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // Toggle flashlight with F key
        if (Input.GetKeyDown(KeyCode.F))
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
        isOn = !isOn;
        flashlight.enabled = isOn;
        detectionCollider.enabled = isOn; // Enable/Disable ghost detection
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
        float batteryPercentage = (batteryLife / 180f) * 100f;

        // Update the text component
        batteryText.text = $"{Mathf.CeilToInt(batteryPercentage)}%";
    }
}
