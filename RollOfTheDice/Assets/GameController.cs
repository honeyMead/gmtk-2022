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

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            ShowWinningMessage();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void ShowWinningMessage()
    {
        ui.SetActive(true);
    }
}
