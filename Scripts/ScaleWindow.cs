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

    [SerializeField]
    Vector3 startPoint = Vector3.zero;

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
                    startPos = main.localPosition;
                }
            }
        }

        if (dragging)
        {
            Vector3 differance = Input.mousePosition - startPoint;
            startPoint = Input.mousePosition;

            main.sizeDelta += (Vector2)differance;

            if (Input.GetMouseButtonUp(0))
            {
                SendMessage("FinishedScalingWindow", main.sizeDelta, SendMessageOptions.DontRequireReceiver);
                dragging = false;
            }
        }
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
