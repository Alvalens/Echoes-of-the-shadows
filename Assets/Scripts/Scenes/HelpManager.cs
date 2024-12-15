using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    public Button nextButton;

    void Start()
    {
        nextButton.onClick.AddListener(GoToGameplay); // Menambahkan listener ke tombol
    }

    void GoToGameplay()
    {
        SceneManager.LoadScene("Main Gameplay"); // Muat scene Gameplay
    }
}
