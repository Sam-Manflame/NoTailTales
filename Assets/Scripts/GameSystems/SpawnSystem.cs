using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField]
    private RectTransform movableArea;
    [SerializeField]
    private RectTransform appearPlace;
    [SerializeField]
    private RectTransform targetPlace;
    [SerializeField]
    private AudioClip printingSound;

    [Header("Common Prefabs")]
    [SerializeField]
    private Image textCardPrefab;
    [SerializeField]
    private Image penaltyPrefab;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public RectTransform spawnPrefab(RectTransform prefab)
    {
        RectTransform obj = Instantiate(prefab, movableArea);
        StartCoroutine(appearRoutine(obj));
        return obj;
    }

    private IEnumerator appearRoutine(RectTransform rect)
    {
        audioSource.clip = printingSound;
        audioSource.Play();

        rect.SetParent(movableArea);
        if (rect.transform.GetComponent<MovableElement>() != null)
            rect.transform.GetComponent<MovableElement>().setCanBeMoved(false);
        rect.localPosition = appearPlace.localPosition - new Vector3(0, rect.rect.height / 2, 0);
        while ((targetPlace.localPosition - rect.localPosition).magnitude > 1)
        {
            rect.localPosition = rect.localPosition + (targetPlace.localPosition - rect.localPosition).normalized * Time.deltaTime * 100;
            yield return new WaitForSeconds(0.001f);
        }
        if (rect.transform.GetComponent<MovableElement>() != null)
            rect.transform.GetComponent<MovableElement>().setCanBeMoved(true);

        audioSource.Stop();
    }

    public void addTextCard(string text, string from)
    {
        Image textCard = spawnPrefab(textCardPrefab.GetComponent<RectTransform>()).GetComponent<Image>();
        textCard.GetComponentsInChildren<Text>()[0].text = string.Format("MESSAGE FROM {0}", from);
        textCard.GetComponentsInChildren<Text>()[1].text = text;
    }

    public void addPenaltyCard(string reason, string animalName, int moneyChange)
    {
        PenaltySetup penaltySetup = spawnPrefab(penaltyPrefab.GetComponent<RectTransform>()).GetComponent<PenaltySetup>();
        penaltySetup.setup(reason, animalName, moneyChange);
    }
}
