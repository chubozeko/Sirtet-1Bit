using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool isGameActive = true;

    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public Piece pieceScript;
    public PieceSpawner spawnSpawner;
    public AudioSource gameOverSound;
    private void Start()
    {
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        CheckForPauseInput();
    }

    private void CheckForPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGameActive)
            {
                PauseGame();
            }
            else
            {
                UnpauseGame();
            }
        }
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        gameOverSound.PlayOneShot(gameOverSound.clip);
    }

    public void PauseGame()
    {
        if (isGameActive)
        {
            isGameActive = false;
            pausePanel.SetActive(true);
            // Stop or pause various game components
            pieceScript.enabled = false;
            spawnSpawner.enabled = false;

            Time.timeScale = 0;
        }
    }

    public void UnpauseGame()
    {
        if (!isGameActive)
        {
            isGameActive = true;
            pausePanel.SetActive(false);
            pieceScript.enabled = true;
            Time.timeScale = 1;
        }
    }
    public void RetryGame()
    {
        SceneManager.LoadSceneAsync(1);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1f;
    }


}
