using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovableElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private string elementId = "undefined";
    [SerializeField]
    private bool canBeRemovedLater = false;
    
    private bool removeable = false;

    private bool move = false;

    private Vector2 prevPos = new Vector2();

    private float moveSpeed = 110f;

    private bool canBeMoved = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canBeMoved)
            return;

        move = true;
        prevPos.Set(eventData.position.x, eventData.position.y);
        //Debug.Log(eventData.position);
        transform.SetAsLastSibling();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        move = false;

        if (canBeRemoved())
        {
            remove();
        }
    }

    void Update()
    {
        if (move)
        {
            Vector3 localPos = Camera.allCameras[0].WorldToScreenPoint(transform.localPosition);
            Vector3 translation = Camera.allCameras[0].ScreenToWorldPoint(new Vector3(localPos.x + (Input.mousePosition.x - prevPos.x) * moveSpeed, localPos.y + (Input.mousePosition.y - prevPos.y) * moveSpeed, localPos.z));
            transform.localPosition = translation;
            prevPos.Set(Input.mousePosition.x, Input.mousePosition.y);
            //Debug.Log(Input.mousePosition);

            
            if (GetComponent<Image>() != null)
            {
                if (canBeRemoved())
                    GetComponent<Image>().color = Color.red;
                else
                    GetComponent<Image>().color = Color.white;
            }
        }
    }

    private bool canBeRemoved()
    {
        if (removeable)
        {
            Vector2 screenPosition = Camera.allCameras[0].WorldToScreenPoint(transform.position);
            Rect cameraRect = FindObjectOfType<Canvas>().pixelRect;
            Rect rect = GetComponent<RectTransform>().rect;
            rect.x = screenPosition.x - rect.width / 2;
            rect.y = screenPosition.y - rect.height / 2;

            Rect r = new Rect(
                Mathf.Max(rect.x, cameraRect.x),
                Mathf.Max(rect.y, cameraRect.y),
                Mathf.Min(rect.x + rect.width, cameraRect.x + cameraRect.width) - Mathf.Max(rect.x, cameraRect.x),
                Mathf.Min(rect.y + rect.height, cameraRect.y + cameraRect.height) - Mathf.Max(rect.y, cameraRect.y));


            return !rect.Equals(r) && r.width * r.height < rect.width * rect.height / 2;
        }
        return false;
    }

    public bool isCanBeRemovedLater()
    {
        return canBeRemovedLater;
    }

    public void setRemoveable(bool val)
    {
        removeable = val;
    }

    private void remove()
    {
        Destroy(gameObject);
    }

    public void setCanBeMoved(bool val)
    {
        canBeMoved = val;
    }
}
