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
        click = GetComponent<AudioSource>();
        if (click == null)
        {
            Debug.LogError("AudioSource component missing on MainMenu!");
        }


         // Tambahkan ini untuk mencegah AudioSource dihancurkan
        DontDestroyOnLoad(gameObject);


        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(ExitGame);

        eventSystem = EventSystem.current;
        pointerData = new PointerEventData(eventSystem);
    }

    void Update()
    {
        // Get joystick input (left stick)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move the cursor based on joystick input
        Vector3 cursorMovement = new Vector3(horizontal, vertical, 0f) * joystickSensitivity;
        if (cursorRect != null)
        {
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
        pointerData.position = cursorRect.position; // Update the pointer position based on cursor

        // Raycast to check if we are hovering over any UI element
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<Button>())
            {
                Button button = result.gameObject.GetComponent<Button>();
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
