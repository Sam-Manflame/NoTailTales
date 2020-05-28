using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public int radioScene;
    public int settingsScene;
    public int creditsScene;

    [SerializeField]
    private Text newGameButtonText;

    private void Start()
    {
        if (PlayerPrefs.GetInt("dayId", 1) > 1)
        {
            newGameButtonText.text = "CONTINUE";
        }
    }

    public void newGamePressed()
    {
        if (PlayerPrefs.GetInt("dayId", -3) == -3)
        {
            PlayerPrefs.SetInt("dayId", 1);
            PlayerPrefs.Save();
        }

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
