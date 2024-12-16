using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText; // Reference to the TextMeshProUGUI component for displaying the final time
    public Button playAgainButton; // Reference to the Play Again button
    public Button mainMenuButton; // Reference to the Main Menu button

    private AudioSource audioSource; // Reference to the AudioSource component

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

        // Add listeners to the buttons
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(MainMenu);
        }

        // Tambahkan ini untuk mencegah AudioSource dihancurkan
        DontDestroyOnLoad(gameObject);
    }

    void PlayAgain()
    {
        PlayClickSound(); // Play the click sound
        PlayerPrefs.DeleteKey("FinalTime");
        SceneManager.LoadScene("Prologue"); // Reload the Prologue scene
    }

    void MainMenu()
    {
        PlayClickSound(); // Play the click sound
        PlayerPrefs.DeleteKey("FinalTime");
        SceneManager.LoadScene("Main Menu"); // Load the Main Menu scene
    }

    private void PlayClickSound()
    {
        if (audioSource != null)
        {
            audioSource.Play(); // Play the sound assigned to the AudioSource
        }
    }
}
