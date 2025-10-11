using System.Drawing;
using UnityEngine;

[RequireComponent (typeof(MouseOverRectTransformPosition), typeof(DragWindow), typeof(ScaleWindow))]
public class DragScaleControl : MonoBehaviour
{
    MouseOverRectTransformPosition mO;
    DragWindow dW;
    ScaleWindow sW;

    [Tooltip("Percent Horizontal range to act as scaling when mouse over furtur inwards will swap to dragging")]
    [Range(0f, 1f)]
    public float scaleIndentRangePercentX = 0.05f;

    [Tooltip("Percent Vertical range to act as scaling when mouse over furtur inwards will swap to dragging")]
    [Range(0f, 1f)]
    public float scaleIndentRangePercentY = 0.05f;

    bool scalingMode = false;
    bool draggingMode = false;

    [Space(10)]
    public bool halfUpdate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mO = GetComponent<MouseOverRectTransformPosition>();
        dW = GetComponent<DragWindow>();
        sW = GetComponent<ScaleWindow>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mO.GetMousePositionPercent(out Vector2 pointPercent, halfUpdate))
        {
            if((pointPercent.x <= scaleIndentRangePercentX || pointPercent.x >= 1 - scaleIndentRangePercentX) ||
                (pointPercent.y <= scaleIndentRangePercentY || pointPercent.y >= 1 - scaleIndentRangePercentY))
            {
                if(!sW.isDragging && !dW.isDragging)
                {
                    scalingMode = true;
                    draggingMode = false;
                }
            }
            else
            {
                if (!sW.isDragging && !dW.isDragging)
                {
                    scalingMode = false;
                    draggingMode = true;
                }
            }
        }
        else
        {
            if (!sW.isDragging && !dW.isDragging)
            {
                scalingMode = false;
                draggingMode = false;
            }
        }

        sW.canDrag = scalingMode;
        dW.canDrag = draggingMode;
    }
}
