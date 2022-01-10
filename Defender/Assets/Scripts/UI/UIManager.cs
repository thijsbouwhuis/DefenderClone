using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    private GameObject gameFinishScreen;

    public void Awake()
    {
        pauseScreen?.SetActive(false);
        gameOverScreen?.SetActive(false);
        gameFinishScreen?.SetActive(false);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseScreen?.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreen?.SetActive(true);
    }

    public void PauseButtonClicked()
    {
        if(Time.timeScale == 1f)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void OpenRestartScreen()
    {
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
    }

    public void RestartGame()
    {
        gameOverScreen.SetActive(false);
        gameFinishScreen.SetActive(false);
        Time.timeScale = 1f;
        GameObject chunkManager = GameObject.FindGameObjectWithTag("ChunkManager");
        chunkManager.GetComponent<ChunkManager>()?.RestartGame();
    }

    public void OpenGameFinishedScreen()
    {
        Time.timeScale = 0f;
        gameFinishScreen.SetActive(true);
    }
}
