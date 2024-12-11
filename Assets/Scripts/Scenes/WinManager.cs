using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText;
    public Button playAgainButton;
    public Button mainMenuButton;
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
