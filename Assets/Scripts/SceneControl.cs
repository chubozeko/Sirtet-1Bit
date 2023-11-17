using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControl : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
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
