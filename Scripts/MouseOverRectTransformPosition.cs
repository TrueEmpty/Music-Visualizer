using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverRectTransformPosition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    public Vector2 localPoint = Vector2.zero;
    public bool isMouseOver = false;

    public Vector2 offset = Vector2.zero;

    [SerializeField]
    Rect pointGrabbed;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isMouseOver)
        {
            Camera uiCamera = (canvas.renderMode == RenderMode.ScreenSpaceCamera) ? canvas.worldCamera : null;

            if (uiCamera != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, uiCamera, out localPoint);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out localPoint);
            }
        }
    }

    public bool GetMousePosition(out Vector2 point)
    {
        point = localPoint;
        return isMouseOver;
    }

    public bool GetMousePositionPercent(out Vector2 pointPercent, bool halfAdjust = false)
    {
        Vector2 nLp = localPoint;

        if (halfAdjust)
        {
            nLp.x += rectTransform.rect.width / 2;
            nLp.y += rectTransform.rect.height / 2;
        }

        nLp += offset;

        pointGrabbed = new Rect(nLp.x, nLp.y, rectTransform.rect.width, rectTransform.rect.height);
        pointPercent = nLp / new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        return isMouseOver;
    }

    public bool GetMousePositionPercent(out Vector2 pointPercent, bool halfAdjustX = false, bool halfAdjustY = false, bool inverseX = false, bool inverseY = false)
    {
        Vector2 nLp = localPoint;

        if (halfAdjustX)
        {
            nLp.x += rectTransform.rect.width / 2;
        }

        if (halfAdjustY)
        {
            nLp.y += rectTransform.rect.height / 2;
        }

        if (inverseX)
        {
            nLp.x *= -1;
        }

        if (inverseY)
        {
            nLp.y *= -1;
        }

        nLp += offset;

        pointGrabbed = new Rect(nLp.x, nLp.y, rectTransform.rect.width, rectTransform.rect.height);
        pointPercent = nLp / new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        return isMouseOver;
    }
}
