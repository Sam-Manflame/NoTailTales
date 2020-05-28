using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RadioSceneController : MonoBehaviour
{
    [SerializeField]
    private int gameScene;
    [SerializeField]
    private Image sceneBackground;
    [SerializeField]
    private float onAirAfter = 1f;
    [SerializeField]
    private Sprite onAirBackground;
    [SerializeField]
    private float messageDelay = 0.5f;
    [SerializeField]
    private Button startDayButton;
    [SerializeField]
    private Button skipButton;
    [SerializeField]
    private Text dayCounter;

    [Header("Audio")]

    [SerializeField]
    private AudioSource radioVoice;
    [SerializeField]
    private AudioSource startNoise;
    [SerializeField]
    private AudioSource shortNoiseSource;
    [SerializeField]
    private AudioSource countrySong;
    
    [SerializeField]
    private List<AudioClip> shortNoises = new List<AudioClip>();
    
    private PrintingTextMessage printingMessage;
    private int messageCounter;
    private Day currentDay;

    private Sprite prevSprite;

    private void Start()
    {
        printingMessage = FindObjectOfType<PrintingTextMessage>();
        printingMessage.GetComponent<Text>().text = "";
        printingMessage.onEnded = () => StartCoroutine(delayedMessage());
        messageCounter = 0;

        try
        {
            currentDay = Day.load(PlayerPrefs.GetInt("dayId"));
        } catch (System.Exception e)
        {
            currentDay = Day.generate(PlayerPrefs.GetInt("dayId"));
        }


        dayCounter.text = "DAY #" + currentDay.id;

        StartCoroutine(onAir());
    }

    private IEnumerator onAir()
    {
        yield return new WaitForSeconds(onAirAfter);

        startNoise.Play();
        StartCoroutine(blinking(onAirAfter, 0.2f));

        yield return new WaitForSeconds(onAirAfter);
        
        sceneBackground.sprite = onAirBackground;
        radioVoice.Play();
        StartCoroutine(timeredNoise(10f));
        nextMessage();

        skipButton.gameObject.SetActive(true);
    }

    private IEnumerator blinking(float blinkingTime, float blinkDelay)
    {
        float time = 0f;
        while (time < blinkingTime)
        {
            time += blinkDelay;
            if (sceneBackground.sprite != onAirBackground)
            {
                prevSprite = sceneBackground.sprite;
                sceneBackground.sprite = onAirBackground;
            } else
            {
                sceneBackground.sprite = prevSprite;
            }
            yield return new WaitForSeconds(blinkDelay);
        }
    }

    private IEnumerator delayedMessage()
    {
        yield return new WaitForSeconds(messageDelay);
        nextMessage();
    }

    private IEnumerator timeredNoise(float timer)
    {
        yield return new WaitForSeconds(Random.RandomRange(3.0f, timer));
        shortNoiseSource.clip = shortNoises[Random.Range(0, shortNoises.Count)];
        shortNoiseSource.Play();
        StartCoroutine(lowVolumeFor(radioVoice, shortNoiseSource.clip.length, 0.0f));
        StartCoroutine(lowVolumeFor(countrySong, shortNoiseSource.clip.length, 0.3f));
        yield return timeredNoise(timer);
    }

    private IEnumerator changeToSong()
    {
        StartCoroutine(timeredNoise(10f));
        
        startNoise.Play();

        float step = 0.007f;
        while (radioVoice.volume > 0.0f)
        {
            radioVoice.volume = Mathf.Max(0.0f, radioVoice.volume - step);
            yield return new WaitForSeconds(0.01f);
        }

        countrySong.volume = 0.0f;
        countrySong.Play();

        while (countrySong.volume < 1.0f)
        {
            countrySong.volume += step;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator lowVolumeFor(AudioSource source, float time, float volume)
    {
        float prevVolume = source.volume;
        source.volume = volume;
        yield return new WaitForSeconds(time);
        source.volume = prevVolume;
    }

    private void nextMessage()
    {
        string message = getMessage(messageCounter);
        if (message == null)
        {
            if (!startDayButton.gameObject.activeSelf)
            {
                StopAllCoroutines();
                StartCoroutine(changeToSong());
            }
            startDayButton.gameObject.SetActive(true);
            
            return;
        }

        printingMessage.setMessage(message);

        messageCounter += 1;
    }

    private string getMessage(int index)
    {
        if (currentDay.radio == null)
            return null;

        if (currentDay.radio.Length > index)
            return currentDay.radio[index];

        return null;
    }

    public void startNewDay()
    {
        SceneManager.LoadScene(gameScene);
    }
}
