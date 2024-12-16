using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button helpButton; // Tombol Help

    void Start()
    {
        helpButton.onClick.AddListener(GoToHelp); // Menambahkan listener ke tombol Help
    }

    void GoToHelp()
    {
        SceneManager.LoadScene("Help Menu"); // Muat scene Help
    }
}
