using UnityEngine;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    RectTransform rectTransform;
    RawImage image;
    Outline outline;
    Shadow shadow;

    AudioManager audioManager;
    public VideoCapture capture;
    public TexturePrint texturePrint;
    MouseOverRectTransformPosition mO;
    DragWindow dragWindow;
    ScaleWindow scaleWindow;
    Border border;

    bool startingSelect = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = AudioManager.instance;    
        
        rectTransform = GetComponent<RectTransform>();
        dragWindow = GetComponent<DragWindow>();
        scaleWindow = GetComponent<ScaleWindow>();
        mO = GetComponent<MouseOverRectTransformPosition>();
        border = GetComponent<Border>();
        image = GetComponent<RawImage>();
        outline = GetComponent<Outline>();
        shadow = GetComponent<Shadow>();

        if(texturePrint.image != null)
        {
            image.texture = texturePrint.image;
        }
        else
        {
            Debug.Log("Was Null Icon");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (capture != null)
        {
            if (texturePrint.WithinTime(capture.currentTime))
            {
                if (texturePrint.image != null && image.texture == null)
                {
                    image.texture = texturePrint.image;
                }

                PositioningAndLyrics();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            image.raycastTarget = !audioManager.unclickables.Contains(gameObject);
            if (texturePrint.WithinTime(audioManager.audioSource.time))
            {
                if (texturePrint.image != null && image.texture == null)
                {
                    image.texture = texturePrint.image;
                }

                Selecting();
                PositioningAndLyrics();
            }
            else
            {
                Destroy(gameObject);
            }
        }        
    }

    void Selecting()
    {
        border.enabled = (audioManager.selectedObject == gameObject);
        dragWindow.canDrag = (audioManager.selectedObject == gameObject);

        Rect rect = transform.parent.GetComponent<RectTransform>().rect;
        Vector2 maxSize = new Vector2(rect.width,rect.height)/2;
        dragWindow.boundry = new Vector4(-maxSize.x, maxSize.x, -maxSize.y, maxSize.y);
        dragWindow.useBoundry = true;

        if (audioManager.selectedObject == null)
        {
            if (Input.GetMouseButtonDown(0) && mO.isMouseOver)
            {
                startingSelect = true;
            }

            if (Input.GetMouseButtonUp(0) && startingSelect)
            {
                if (mO.isMouseOver)
                {
                    audioManager.selectedObject = gameObject;
                }

                startingSelect = false;
            }

            if (!Input.GetMouseButton(0))
            {
                startingSelect = false;
            }
        }
        else if (audioManager.selectedObject == gameObject)
        {
            if (Input.GetMouseButtonDown(2))
            {
                audioManager.unclickables.Add(gameObject);
                audioManager.selectedObject = null;
            }
        }
    }

    void PositioningAndLyrics()
    {
        if (texturePrint.image != null && image.texture == null)
        {
            image.texture = texturePrint.image;
        }

        if (!dragWindow.isDragging)
        {
            rectTransform.anchoredPosition = texturePrint.position;
        }

        if (!scaleWindow.isDragging)
        {
            rectTransform.sizeDelta = texturePrint.size;
        }
    }

    void FinishedDraggingWindow(Vector2 newPos)
    {
        texturePrint.position = newPos;
    }

    void FinishedScalingWindow((Vector2, Vector2) newSizePos)
    {
        texturePrint.size = newSizePos.Item1;
        texturePrint.position = newSizePos.Item2;
    }
}
