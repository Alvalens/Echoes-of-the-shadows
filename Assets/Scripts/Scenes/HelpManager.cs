using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    public Button nextButton; // Reference to the Next button
    private AudioSource audioSource; // Reference to the AudioSource component

    void Start()
    {
        // Automatically find and initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found! Please attach an AudioSource component.");
        }

        // Prevent AudioSource from being destroyed when scenes change
        DontDestroyOnLoad(gameObject);

        nextButton.onClick.AddListener(GoToGameplay); // Add listener to the button

    }

    void GoToGameplay()
    {

        PlayClickSound(); // Play the click sound
        SceneManager.LoadScene("Main Gameplay"); // Load the Main Gameplay scene
    }

    private void PlayClickSound()
    {
        if (audioSource != null)
        {
            audioSource.Play(); // Play the sound assigned to the AudioSource
        }
    }
}
