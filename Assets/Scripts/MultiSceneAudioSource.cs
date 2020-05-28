using UnityEngine;

public class MultiSceneAudioSource : MonoBehaviour
{
    [SerializeField]
    private string playerPrefsId = "undefinded_sound";

    private void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.time = PlayerPrefs.GetFloat(playerPrefsId, 0);
        audioSource.Play();
    }

    private void Update()
    {
        PlayerPrefs.SetFloat(playerPrefsId, GetComponent<AudioSource>().time);
        PlayerPrefs.Save();
    }

    public string getPlayerPrefsId()
    {
        return playerPrefsId;
    }
}
