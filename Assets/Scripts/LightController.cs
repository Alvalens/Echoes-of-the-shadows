using UnityEngine;

public class LightController : MonoBehaviour
{
    private Light pointLight;

    void Start()
    {
        // Get the Light component on the same GameObject
        pointLight = GetComponent<Light>();
    }

    public void ToggleLight(bool state)
    {
        // Enable or disable the light
        if (pointLight != null)
        {
            pointLight.enabled = state;
        }
    }
}
