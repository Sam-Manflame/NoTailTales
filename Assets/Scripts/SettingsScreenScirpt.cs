using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsScreenScirpt : MonoBehaviour
{
    public int menuScene = 0;

    public void resetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void backToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }
}
