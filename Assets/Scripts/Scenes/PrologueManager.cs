using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    public Button nextButton;

    void Start()
    {
        nextButton.onClick.AddListener(GoToGameplay); // Add listener
    }
    void GoToGameplay()
    {
        SceneManager.LoadScene("Main Gameplay"); // Load main gameplay
    }
}
