using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private AudioSource click;
    public Button newGameButton;
    public Button continueButton;
    public Button exitButton;

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
