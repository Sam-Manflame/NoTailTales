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

    [Header("Common Prefabs")]
    [SerializeField]
    private Image textCardPrefab;
    [SerializeField]
    private Image penaltyPrefab;

    public RectTransform spawnPrefab(RectTransform prefab)
    {
        RectTransform obj = Instantiate(prefab, movableArea);
        StartCoroutine(appearRoutine(obj));
        return obj;
    }

    private IEnumerator appearRoutine(RectTransform rect)
    {
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
    }

    public void addTextCard(string text, string from)
    {
        Image textCard = spawnPrefab(textCardPrefab.GetComponent<RectTransform>()).GetComponent<Image>();
        textCard.GetComponentsInChildren<Text>()[0].text = string.Format("MESSAGE FROM {0}", from);
        textCard.GetComponentsInChildren<Text>()[1].text = text;
    }

    public void addPenaltyCard(string reason, int moneyChange)
    {
        spawnPrefab(penaltyPrefab.GetComponent<RectTransform>());
    }
}
