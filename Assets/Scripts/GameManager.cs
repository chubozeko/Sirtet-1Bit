using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private bool isGameActive = true;
    private bool isPaused = false;

    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public PieceSpawner spawnSpawner;
    public AudioSource gameOverSound;
    public AudioSource winSound;
    public Text winConditionText;
    public Text t_GO_Points;
    public Text t_LC_Points;
    private int winCondition = 10;
    public UnityEvent OnGameOver = new UnityEvent();

    public Grid grid;

    private void Start()
    {
        OnGameOver.AddListener(GameOver);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        UpdateWinConditionText();
    }

    private void Update()
    {
        CheckForPauseInput();
        CheckForWinCondition();
    }

    private void CheckForWinCondition()
    {
        if (winCondition <= 0)
        {
            isGameActive = false;
            isPaused = false;
            winPanel.SetActive(true);
            winSound.PlayOneShot(winSound.clip);
        }
    }

    private void UpdateWinConditionText()
    {
        if (winConditionText != null)
        {
            winConditionText.text = winCondition.ToString();
        }
    }

    private void CheckForPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGameActive)
            {
                TogglePause();
            }
            else
            {
                UnpauseGame();
            }
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        isPaused = false;
        t_GO_Points.text = "Total Points: " + grid.GetTotalPoints().ToString();
        gameOverPanel.SetActive(true);
        gameOverSound.PlayOneShot(gameOverSound.clip);
        // Deactivate running components
        spawnSpawner.enabled = false;
        grid.activePiece.SetActive(false);
    }

    public void LevelComplete()
    {
        isGameActive = false;
        isPaused = false;
        t_LC_Points.text = "Total Points: " + grid.GetTotalPoints().ToString();
        winPanel.SetActive(true);
        winSound.PlayOneShot(winSound.clip);
        grid.activePiece.SetActive(false);
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }

    private void PauseGame()
    {
        if (isGameActive && !isPaused)
        {
            isPaused = true;
            pausePanel.SetActive(true);
            // Stop or pause various game components
            spawnSpawner.enabled = false;
            grid.activePiece.SetActive(false);
        }
    }

    private void UnpauseGame()
    {
        if (isGameActive && isPaused)
        {
            isPaused = false;
            pausePanel.SetActive(false);
            // Resume or start various game components
            spawnSpawner.enabled = true;
            grid.activePiece.SetActive(true);
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        isGameActive = true;
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        isGameActive = true;
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            print("Exit");
        #endif
            Application.Quit();
    }


}