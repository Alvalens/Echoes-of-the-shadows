using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackCreditsManager : MonoBehaviour
{
    public Button BackButton; // Tombol Credits
    public float joystickSensitivity = 10f; // Joystick movement sensitivity
    public RectTransform cursorRect; // UI element representing the cursor (optional)
    public string interactButtonName = "Interact"; // Interact button name (mapped in Input Manager)
    private PointerEventData pointerData;
    private EventSystem eventSystem;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        BackButton.onClick.AddListener(GoToCredits); // Tambahkan listener ke tombol Credits

        // Initialize EventSystem and PointerEventData for cursor interactions
        eventSystem = EventSystem.current;
        pointerData = new PointerEventData(eventSystem);

        // Hide cursor rect by default
        if (cursorRect != null)
        {
            cursorRect.gameObject.SetActive(false);
        }

        // Subscribe to controller connection events
        if (GameControllers.Instance != null)
        {
            GameControllers.Instance.OnControllerConnectedEvent += HandleControllerConnected;
            GameControllers.Instance.OnControllerDisconnectedEvent += HandleControllerDisconnected;

            // Check initial state
            UpdateCursorVisibility(GameControllers.Instance.IsControllerConnected);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (GameControllers.Instance != null)
        {
            GameControllers.Instance.OnControllerConnectedEvent -= HandleControllerConnected;
            GameControllers.Instance.OnControllerDisconnectedEvent -= HandleControllerDisconnected;
        }
    }

    void HandleControllerConnected()
    {
        UpdateCursorVisibility(true);
    }

    void HandleControllerDisconnected()
    {
        UpdateCursorVisibility(false);
    }

    void UpdateCursorVisibility(bool isConnected)
    {
        if (cursorRect != null)
        {
            cursorRect.gameObject.SetActive(isConnected);
        }
    }

    void Update()
    {
        // Only process cursor movement if controller is connected
        if (GameControllers.Instance != null && GameControllers.Instance.IsControllerConnected)
        {
            // Get joystick input (left stick)
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Move the cursor based on joystick input
            if (cursorRect != null)
            {
                Vector3 cursorMovement = new Vector3(horizontal, vertical, 0f) * joystickSensitivity;
                cursorRect.anchoredPosition += new Vector2(cursorMovement.x, cursorMovement.y);
            }

            // Simulate a click if the interact button is pressed
            if (Input.GetButtonDown(interactButtonName))
            {
                SimulateClick();
            }
        }
    }
    void SimulateClick()
    {
        if (cursorRect == null) return;

        pointerData.position = cursorRect.position; // Update the pointer position based on cursor

        // Raycast to check if we are hovering over any UI element
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke(); // Simulate button click
                break; // Only click one button at a time
            }
        }
    }

    void GoToCredits()
    {
        SceneManager.LoadScene("Main Menu"); // Muat scene Credits
    }
}
