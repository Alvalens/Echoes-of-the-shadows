using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;          // Reference to the Spotlight component
    public float batteryLife = 180f;  // 3 minutes of battery life
    public bool isOn = true;          // Flashlight state
    private Collider detectionCollider; // The trigger collider to detect ghosts
    public TextMeshProUGUI batteryText;

    void Start()
    {
        // Get the Light and Collider components
        if (!flashlight) flashlight = GetComponent<Light>();
        detectionCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // Toggle flashlight with F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }

        // Drain battery if the flashlight is on
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

        // Debug ghost detection
        DetectGhost();
        UpdateBatteryText();
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

    void DetectGhost()
    {
        if (detectionCollider.enabled)
        {
            // Check for collisions with ghosts
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionCollider.bounds.extents.magnitude);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("ghost"))
                {
                    Debug.Log("Ghost detected!");
                }
            }
        }
    }

    void UpdateBatteryText()
    {
        // Calculate battery percentage
        float batteryPercentage = (batteryLife / 180f) * 100f;

        // Update the text component
        batteryText.text = $"Battery: {Mathf.CeilToInt(batteryPercentage)}%";
    }
}
