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

    [SerializeField]
    bool scalingMode = false;

    [SerializeField]
    bool draggingMode = false;

    [Space(10)]
    public bool directionalScaling = true;
    public bool halfUpdate = false;

    public bool lockHoizontal = false;
    public bool lockVertical = false;

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
        (bool, bool, bool, bool) scaleDirection = (true, true, true, true);

        if(mO.GetMousePositionPercent(out Vector2 pointPercent, halfUpdate))
        {
            if((pointPercent.x <= scaleIndentRangePercentX || pointPercent.x >= 1 - scaleIndentRangePercentX) ||
                (pointPercent.y <= scaleIndentRangePercentY || pointPercent.y >= 1 - scaleIndentRangePercentY))
            {
                if(!sW.isDragging && !dW.isDragging)
                {
                    scalingMode = true;
                    draggingMode = false;

                    if (directionalScaling)
                    {
                        //Top
                        scaleDirection.Item1 = (pointPercent.y >= 1 - scaleIndentRangePercentY);
                        //Right
                        scaleDirection.Item2 = (pointPercent.x >= 1 - scaleIndentRangePercentX);
                        //Bot
                        scaleDirection.Item3 = (pointPercent.y <= scaleIndentRangePercentY);
                        //Left
                        scaleDirection.Item4 = (pointPercent.x <= scaleIndentRangePercentX);
                    }
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

        sW.lockHoizontal = lockHoizontal;
        sW.lockVertical = lockVertical;
        sW.canDrag = scalingMode;
        if(!sW.isDragging)
        {
            sW.SetScaleDirections(scaleDirection);
        }
        dW.canDrag = draggingMode;
        dW.lockHoizontal = lockHoizontal;
        dW.lockVertical = lockVertical;
    }

    public bool Scaling()
    {
        return scalingMode;
    }

    public bool Dragging()
    {
        return draggingMode;
    }
}
