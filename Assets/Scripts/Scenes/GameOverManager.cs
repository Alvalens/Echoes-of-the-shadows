using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public Button playAainButton;
    public Button mainMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playAainButton.onClick.AddListener(PlayAgain);
        mainMenuButton.onClick.AddListener(MainMenu);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayAgain()
    {
        SceneManager.LoadScene("Prologue");
    }

    void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
