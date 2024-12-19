// File: HelpMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpMenuManager : MonoBehaviour
{
    public Button backButton; // Tombol Back

    void Start()
    {
        backButton.onClick.AddListener(GoToMenu); // Menambahkan listener ke tombol Back
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("Main Menu"); // Muat scene Main Menu
    }
}
