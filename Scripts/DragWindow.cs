using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform main = null;
    [Tooltip("Min X, Max X, Min Y, Max Y")]
    public Vector4 boundry = new Vector4(0,0,0,0);
    public bool useBoundry = false;
    Vector2 adjust = Vector2.one;

    [SerializeField]
    bool mouseOver = false;

    [SerializeField]
    bool dragging = false;

    [SerializeField]
    Vector3 startPoint = Vector3.zero;

    [SerializeField]
    Vector3 startPos = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(main == null) 
        {
            main = transform.parent.GetComponent<RectTransform>();
        }

        adjust.x = Screen.width/1920;
        adjust.y = Screen.height/962;
    }

    // Update is called once per frame
    void Update()
    {
        if(mouseOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragging = true;
                startPoint = Input.mousePosition;
                startPos = main.localPosition;
            }
        }

        if(dragging)
        {
            Vector3 differance = Input.mousePosition - startPoint;
            Vector3 newPos = startPos + differance;

            if(useBoundry)
            {
                if(newPos.x < boundry.x * adjust.x)
                {
                    newPos.x = boundry.x * adjust.x;
                }
                else if(newPos.x > boundry.y * adjust.x)
                {
                    newPos.x = boundry.y * adjust.x;
                }


                if (newPos.y < boundry.z * adjust.y)
                {
                    newPos.y = boundry.z * adjust.y;
                }
                else if (newPos.y > boundry.w * adjust.y)
                {
                    newPos.y = boundry.w * adjust.y;
                }
            }

            main.localPosition = newPos;

            if (Input.GetMouseButtonUp (0))
            {
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
}
