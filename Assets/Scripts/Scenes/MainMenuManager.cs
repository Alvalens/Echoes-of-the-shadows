using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button newGameButton;
    public Button continueButton;
    public Button exitButton;

    void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    void ContinueGame()
    {
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
        PlayerPrefs.DeleteAll(); // Clear any saved data
        SceneManager.LoadScene("Prologue"); // Replace with the actual scene name
    }

    void ExitGame()
    {
        Application.Quit();
    }
}
