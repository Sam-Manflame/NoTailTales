using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private Text dayCounter;
    
    private PrintingTextMessage printingMessage;
    private int messageCounter;
    private Day currentDay;

    private void Start()
    {
        printingMessage = FindObjectOfType<PrintingTextMessage>();
        printingMessage.GetComponent<Text>().text = "";
        printingMessage.onEnded = () => StartCoroutine(delayedMessage());
        messageCounter = 0;
        currentDay = Day.load(PlayerPrefs.GetInt("dayId"));
        dayCounter.text = "DAY #" + currentDay.id;

        StartCoroutine(onAir());
    }

    private IEnumerator onAir()
    {
        yield return new WaitForSeconds(onAirAfter);
        sceneBackground.sprite = onAirBackground;
        nextMessage();
    }

    private IEnumerator delayedMessage()
    {
        yield return new WaitForSeconds(messageDelay);
        nextMessage();
    }

    private void nextMessage()
    {
        string message = getMessage(messageCounter);
        if (message == null)
        {
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
