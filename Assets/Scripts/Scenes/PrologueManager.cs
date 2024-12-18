using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PrologueManager : MonoBehaviour
{
    public Button nextButton; // Reference to the Next button
    public TextMeshProUGUI textDisplay; // Reference to the TextMeshProUGUI component
    private AudioSource audioSource; // Reference to the AudioSource component

    public float joystickSensitivity = 10f; // Joystick movement sensitivity
    public RectTransform cursorRect; // UI element representing the cursor (optional)
    public string interactButtonName = "Interact"; // Interact button name (mapped in Input Manager)
    private PointerEventData pointerData;
    private EventSystem eventSystem;

    private string[] texts = new string[]
        {
        "Aku berjalan melewati hutan yang sunyi dan menemukan rumah itu. Rumah tua yang diwariskan oleh kerabat yang hampir tak kuingat. Tugasku hanya <color=yellow>membersihkannya</color>, menyiapkannya untuk dijual, dan meninggalkannya untuk selamanya.",
        "Ketika aku membuka pintu, bau debu langsung tercium. Cahaya dari senterku menyinari sudut-sudut gelap, membuat bayangan aneh di dinding. Namun, aku menyadari ada masalah lain, <color=yellow>kadang-kadang listrik mati secara tiba-tiba.</color> Di luar rumah, aku menemukan sebuah generator tua. Sepertinya aku harus menggunakannya jika lampu padam.",
        "\"Jangan tinggal sampai malam,\" kata seorang penduduk desa memperingatkan. \"Rumah itu tidak suka tamu, dan mereka bilang <color=yellow>cahaya adalah satu-satunya hal yang bisa membuatnya menjauh.</color>\" Kini, saat bayangan semakin panjang, perasaan tak nyaman mulai muncul.",
        "Aku melihat sebuah jam tanganku, jarumnya berdetak pelan menuju tengah malam. <color=yellow>Jam menunjukkan pukul 03:00 adalah batas waktuku.</color> Aku harus menyelesaikan semuanya sebelum itu, atau mungkin aku tidak akan pernah keluar dari sini dengan selamat."
        };
    private int currentTextIndex = 0; // Keeps track of which text to display

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Automatically find and initialize components
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found! Please attach an AudioSource component.");
        }

        // Prevent AudioSource from being destroyed
        // DontDestroyOnLoad(audioSource);

        // Initialize event system and pointer data for cursor interaction
        eventSystem = EventSystem.current;
        pointerData = new PointerEventData(eventSystem);

        textDisplay.text = texts[currentTextIndex]; // Display the first text
        nextButton.onClick.AddListener(NextText); // Add listener to the button

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

    void NextText()
    {
        PlayClickSound(); // Play the click sound
        currentTextIndex++;
        if (currentTextIndex < texts.Length)
        {
            textDisplay.text = texts[currentTextIndex]; // Update the displayed text
        }
        else
        {
            SceneManager.LoadScene("Help"); // Load the next scene when all texts are shown
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