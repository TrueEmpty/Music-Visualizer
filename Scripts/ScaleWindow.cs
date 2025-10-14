using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform main = null;

    [SerializeField]
    bool mouseOver = false;

    [SerializeField]
    bool dragging = false;

    public bool canDrag = true;

    public bool dragFromTop = true;
    public bool dragFromRight = true;
    public bool dragFromBot = true;
    public bool dragFromLeft = true;

    public bool lockHoizontal = false;
    public bool lockVertical = false;

    public Vector4 positionStable = new Vector4(.5f, .5f, .5f, .5f);//Top,Right,Bot,Left


    [SerializeField]
    Vector3 startPoint = Vector3.zero;

    [SerializeField]
    Vector2 startSize;

    [SerializeField]
    Vector3 startPos = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(main == null) 
        {
            main = transform.GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(canDrag)
        {
            if (mouseOver)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    dragging = true;
                    startPoint = Input.mousePosition;
                    startSize = main.sizeDelta;
                    startPos = main.localPosition;
                }
            }
        }

        if (dragging)
        {
            Vector3 delta = Input.mousePosition - startPoint;
            Vector2 newSize = startSize;
            Vector2 newPos = startPos;

            if (!lockVertical)
            {
                // Vertical scaling
                if (dragFromTop && dragFromBot)
                {
                    newSize.y = startSize.y + delta.y;
                }
                else if (dragFromTop)
                {
                    newSize.y = startSize.y + delta.y;
                    newPos.y = startPos.y + delta.y * positionStable.x;
                }
                else if (dragFromBot)
                {
                    newSize.y = startSize.y - delta.y;
                    newPos.y = startPos.y + delta.y * positionStable.z;
                }
            }

            // Horizontal scaling
            if(!lockHoizontal)
            {
                if (dragFromLeft && dragFromRight)
                {
                    newSize.x = startSize.x + delta.x;
                }
                else if (dragFromRight)
                {
                    newSize.x = startSize.x + delta.x;
                    newPos.x = startPos.x + delta.x * positionStable.y;
                }
                else if (dragFromLeft)
                {
                    newSize.x = startSize.x - delta.x;
                    newPos.x = startPos.x + delta.x * positionStable.w;
                }
            }

            main.sizeDelta = newSize;
            main.localPosition = newPos;

            if (Input.GetMouseButtonUp(0))
            {
                SendMessage("FinishedScalingWindow", (main.sizeDelta, main.anchoredPosition), SendMessageOptions.DontRequireReceiver);
                dragging = false;
            }
        }
    }

    public void SetScaleDirections(bool top, bool right, bool bot, bool left)
    {
        dragFromTop = top;
        dragFromRight = right;
        dragFromBot = bot;
        dragFromLeft = left;
    }

    public void SetScaleDirections((bool, bool, bool, bool) scaleDirectionsTRBL)
    {
        dragFromTop = scaleDirectionsTRBL.Item1;
        dragFromRight = scaleDirectionsTRBL.Item2;
        dragFromBot = scaleDirectionsTRBL.Item3;
        dragFromLeft = scaleDirectionsTRBL.Item4;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mouseOver = false;
    }

    public bool isMouseOver 
    {
        get 
        {
            return mouseOver;
        }
    }

    public bool isDragging
    {
        get
        {
            return dragging;
        }
    }
}
