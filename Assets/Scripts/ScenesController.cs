using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject tutorialScreen;

    public bool isPaused = false;
    public bool isTutorial = false;

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void GoToMenu()
    {

        SceneManager.LoadScene("MainMenuScene");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    public void GameWon()
    {
        SceneManager.LoadScene("WinScene");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowTutorial()
    {
        tutorialScreen.SetActive(true);
        isTutorial = true;
    }

    public void CloseTutorial()
    {
        tutorialScreen.SetActive(false);
        isTutorial = false;
    }

    public void Resume()
    {
        if (isTutorial) CloseTutorial();

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
