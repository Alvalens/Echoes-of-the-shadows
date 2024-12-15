using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    public Button nextButton;

    void Start()
    {
        nextButton.onClick.AddListener(GoToHelp); // Menambahkan listener ke tombol
    }

    void GoToHelp()
    {
        SceneManager.LoadScene("Help"); // Muat scene Help
    }
}
