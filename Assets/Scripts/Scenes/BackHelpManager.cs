using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackHelpManager : MonoBehaviour
{
    public Button backButton; // Tombol Back

    void Start()
    {
        backButton.onClick.AddListener(GoToMenu); // Menggunakan GoToMenu yang sudah didefinisikan
    }

    // Perbaikan: Ganti nama fungsi menjadi GoToMenu untuk konsistensi
    void GoToMenu()
    {
        SceneManager.LoadScene("Prologue"); // Muat scene Prologue
    }
}
