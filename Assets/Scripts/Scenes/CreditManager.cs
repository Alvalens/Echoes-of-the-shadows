using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    public Button CreditsButton; // Tombol Credits

    void Start()
    {
        CreditsButton.onClick.AddListener(GoToCredits); // Tambahkan listener ke tombol Credits
    }

    void GoToCredits()
    {
        SceneManager.LoadScene("Credits"); // Muat scene Credits
    }
}
