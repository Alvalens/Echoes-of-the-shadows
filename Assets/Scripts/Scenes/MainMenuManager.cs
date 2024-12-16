using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    private AudioSource click;
    public Button newGameButton;
    public Button continueButton;
    public Button exitButton;
    public float joystickSensitivity = 10f; // Joystick movement sensitivity
    public RectTransform cursorRect; // UI element representing the cursor (optional)
    public string interactButtonName = "Interact"; // Interact button name (mapped in Input Manager)
    private PointerEventData pointerData;
    private EventSystem eventSystem;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        click = GetComponent<AudioSource>();
        if (click == null)
        {
            Debug.LogError("AudioSource component missing on MainMenu!");
        }

        // Prevent AudioSource from being destroyed
        DontDestroyOnLoad(gameObject);

        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(ExitGame);
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

    // Simulate button click when the interact button is pressed
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
                PlayClickSound(); // Play sound when a button is clicked
                break; // Only click one button at a time
            }
        }
    }

    void ContinueGame()
    {
        PlayClickSound();
        if (PlayerPrefs.HasKey("CleanableState"))
        {
            // Load the last scene or start the saved game
            SceneManager.LoadScene(PlayerPrefs.GetString("LastScene"));
        }
        else
        {
            Debug.Log("No saved game found!");
        }
    }

    void NewGame()
    {
        PlayClickSound();
        PlayerPrefs.DeleteAll(); // Clear any saved data
        SceneManager.LoadScene("Prologue"); // Replace with the actual scene name
    }

    void ExitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    void PlayClickSound()
    {
        if (click != null)
        {
            click.Play();
        }
    }
}