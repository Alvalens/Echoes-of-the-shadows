using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GameOverManager : MonoBehaviour
{

    public Button playAgainButton;
    public Button mainMenuButton;
    public Button playAtCheckPointButton;
    public GameObject loadingScreen; // Reference to the loading screen UI
    public TextMeshProUGUI loadingText; // Reference to t

    // Joystick settings for cursor control
    public float joystickSensitivity = 10f;
    public RectTransform cursorRect; // Reference to the cursor UI element
    public string interactButtonName = "Interact"; // Interact button name (mapped in Input Manager)

    private AudioSource audioSource; // Reference to the AudioSource component
    private PointerEventData pointerData;
    private EventSystem eventSystem;
 

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Automatically find and initialize the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found! Please attach an AudioSource component.");
        }

        // change this method, its keep all alive not specific object such as audio
        //DontDestroyOnLoad(gameObject);

        // Button listeners
        playAgainButton.onClick.AddListener(PlayAgain);
        mainMenuButton.onClick.AddListener(MainMenu);
        playAtCheckPointButton.onClick.AddListener(PlayAtCheckPoint);

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

    // Update is called once per frame
    void Update()
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

    // Simulate button click when the interact button is pressed
    void SimulateClick()
    {
        if (cursorRect == null) return;

        pointerData.position = cursorRect.position; // Update the pointer position based on cursor

        // Raycast to check if we are hovering over any UI element
        var raycastResults = new List<RaycastResult>();
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

    void PlayAgain()
    {
        PlayClickSound(); // Play the click sound
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Prologue"); // Load the Prologue scene
    }

    void MainMenu()
    {

        PlayClickSound(); // Play the click sound
        SceneManager.LoadScene("Main Menu"); // Load the Main Menu scene
    }

    void PlayAtCheckPoint()
    {
        PlayClickSound(); // Play the click sound
        StartCoroutine(LoadSceneAsync("Main Gameplay"));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // Show the loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // While the scene is loading, display the loading text
        while (!operation.isDone)
        {
            if (loadingText != null)
            {
                loadingText.text = "Loading...";
            }
            yield return null;
        }

        // Hide the loading screen once the scene is loaded
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null)
        {
            audioSource.Play(); // Play the sound assigned to the AudioSource
        }
    }
}
