using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public int radioScene;
    public int settingsScene;
    public int creditsScene;

    private void Start()
    {
        // TODO: change NEW GAME to CONTINUE
    }

    public void newGamePressed()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("dayId", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(radioScene);
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
