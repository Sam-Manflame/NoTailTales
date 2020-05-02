using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public int gameScene;
    public int settingsScene;
    public int creditsScene;

    private void Start()
    {
        // TODO: change NEW GAME to CONTINUE
    }

    public void newGamePressed()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void settingsPressed()
    {
        SceneManager.LoadScene(settingsScene);
    }

    public void creditsPressed()
    {
        SceneManager.LoadScene(creditsScene);
    }

    public void exitPressed()
    {
        Application.Quit();
    }
}
