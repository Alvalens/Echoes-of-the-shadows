using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackCreditsManager : MonoBehaviour
{
    public Button BackButton; // Tombol Credits

    void Start()
    {
        BackButton.onClick.AddListener(GoToCredits); // Tambahkan listener ke tombol Credits
    }

    void GoToCredits()
    {
        SceneManager.LoadScene("Main Menu"); // Muat scene Credits
    }
}
