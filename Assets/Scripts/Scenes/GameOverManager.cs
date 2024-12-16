using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
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

        // Tambahkan ini untuk mencegah AudioSource dihancurkan
        DontDestroyOnLoad(gameObject);

        playAgainButton.onClick.AddListener(PlayAgain);
        mainMenuButton.onClick.AddListener(MainMenu);
    }

    void PlayAgain()
    {
        PlayClickSound(); // Play the click sound
        SceneManager.LoadScene("Prologue"); // Load the Prologue scene
    }

    void MainMenu()
    {
        PlayClickSound(); // Play the click sound
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
