using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class WinManager : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText;
    public Button playAgainButton;
    public Button mainMenuButton;

    public float joystickSensitivity = 10f; // Joystick movement sensitivity
    public RectTransform cursorRect; // UI element representing the cursor (optional)
    public string interactButtonName = "Interact"; // Interact button name (mapped in Input Manager)
    private PointerEventData pointerData;
    private EventSystem eventSystem;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (PlayerPrefs.HasKey("FinalTime"))
        {
            float finalTime = PlayerPrefs.GetFloat("FinalTime");
            int minutes = Mathf.FloorToInt(finalTime / 60);
            int seconds = Mathf.FloorToInt(finalTime % 60);

            // Display the final time
            if (finalTimeText != null)
            {
                finalTimeText.text = $"You completed the game in {minutes:00}:{seconds:00}!";
            }
        }
        else
        {
            Debug.LogWarning("No final time found in PlayerPrefs!");
        }

        // Add a listener to the play again button
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
        }
        if (mainMenuButton != null) {
            mainMenuButton.onClick.AddListener(MainMenu);
        }

        // Initialize event system and pointer data for cursor interaction
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
    void PlayAgain()
    {
        PlayerPrefs.DeleteKey("FinalTime");
        // Reload the current scene
        SceneManager.LoadScene("Prolouge");
    }

    void MainMenu()
    {
        PlayerPrefs.DeleteKey("FinalTime");
        // Load the main menu scene
        SceneManager.LoadScene("Main Menu");
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
                //PlayClickSound(); // Play sound when a button is clicked
                break; // Only click one button at a time
            }
        }
    }
    // Update is called once per frame
    void Update()
        {
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
}
