using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private GameObject ui;
    private static GameObject permanentMusic = null;
    private AudioSource sfx;

    void Start()
    {
        ui = GameObject.FindWithTag("UI");
        ui.SetActive(false);
        sfx = GetComponent<AudioSource>();

        if (permanentMusic == null)
        {
            permanentMusic = GameObject.FindWithTag("Music");
            if (permanentMusic != null)
            {
                DontDestroyOnLoad(permanentMusic);
            }
        }
        else
        {
            RemoveDoubledMusic();
        }
    }

    public void WinScene()
    {
        sfx.Play();
        Time.timeScale = 0.2f;
        StartCoroutine(Waiter(() => LoadNextLevel()));
    }

    private void LoadNextLevel()
    {
        var currentLevel = SceneManager.GetActiveScene().buildIndex;
        var nextLevel = currentLevel + 1;

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

    public void RestartLevel()
    {
        var currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void RemoveDoubledMusic()
    {
        var musics = GameObject.FindGameObjectsWithTag("Music");

        foreach (var music in musics)
        {
            if (music != permanentMusic)
            {
                music.SetActive(false);
            }
        }
    }

    private IEnumerator Waiter(Action action)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        action();
    }
}
