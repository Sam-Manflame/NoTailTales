using System.Collections;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField]
    private RectTransform movableArea;
    [SerializeField]
    private RectTransform appearPlace;
    [SerializeField]
    private RectTransform targetPlace;

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
}
