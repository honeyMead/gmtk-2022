using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private GameObject ui;

    void Start()
    {
        ui = GameObject.FindWithTag("UI");
        ui.SetActive(false);
    }

    public void LoadNextLevel()
    {
        var currentLevel = SceneManager.GetActiveScene().buildIndex;
        var nextLevel = currentLevel + 1;

        // TODO play sound
        Time.timeScale = 0.2f;

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(Waiter(() => LoadNextScene(nextLevel)));
        }
        else
        {
            StartCoroutine(Waiter(() => ShowWinningMessage()));
        }
    }

    private void LoadNextScene(int nextLevel)
    {
        SceneManager.LoadScene(nextLevel);
        Time.timeScale = 1;
    }

    private void ShowWinningMessage()
    {
        ui.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    private IEnumerator Waiter(Action action)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        action();
    }
}
