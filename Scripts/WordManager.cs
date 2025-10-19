using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class WordManager : MonoBehaviour
{
    RectTransform rectTransform;
    Text word;
    Outline outline;
    Shadow shadow;

    AudioManager audioManager;
    public VideoCapture capture;
    public LyricLine lyricLine;
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
        word = GetComponent<Text>();
        outline = GetComponent<Outline>();
        shadow = GetComponent<Shadow>();

        PositioningAndLyrics();
    }

    // Update is called once per frame
    void Update()
    {
        if (capture != null)
        {
            border.enabled = false;
            
            if (lyricLine.WithinTime(capture.currentTime))
            {
                PositioningAndLyrics();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            word.raycastTarget = !audioManager.unclickables.Contains(gameObject);
            if (lyricLine.WithinTime(audioManager.audioSource.time))
            {
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
        border.effectColor = (audioManager.currentAutoLyric == this) ? audioManager.autoLyricColor : audioManager.borderColor;
        dragWindow.canDrag = (audioManager.selectedObject == gameObject);

        Rect rect = transform.parent.GetComponent<RectTransform>().rect;
        Vector2 maxSize = new Vector2(rect.width, rect.height)/2;
        dragWindow.boundry = new Vector4(-maxSize.x, maxSize.x,-maxSize.y, maxSize.y);
        dragWindow.useBoundry = true;

        if (audioManager.selectedObject == null)
        {
            if (Input.GetMouseButtonDown(0) && mO.isMouseOver)
            {
                startingSelect = true;
            }

            if(Input.GetMouseButtonUp(0) && startingSelect)
            {
                if(mO.isMouseOver)
                {
                    audioManager.selectedObject = gameObject;
                }

                startingSelect = false;
            }

            if(!Input.GetMouseButton(0))
            {
                startingSelect = false;
            }
        }
        else if(audioManager.selectedObject == gameObject)
        {
            if(Input.GetMouseButtonDown(2))
            {
                audioManager.unclickables.Add(gameObject);
                audioManager.selectedObject = null;
            }
        }
    }

    void PositioningAndLyrics()
    {
        word.text = lyricLine.text;

        if (!dragWindow.isDragging)
        {
            rectTransform.anchoredPosition = lyricLine.position;
        }

        if (!scaleWindow.isDragging)
        {
            rectTransform.sizeDelta = lyricLine.size;
        }
    }

    void FinishedDraggingWindow(Vector2 newPos)
    {
        lyricLine.position = newPos;
    }

    void FinishedScalingWindow((Vector2,Vector2) newSizePos)
    {
        lyricLine.size = newSizePos.Item1;
        lyricLine.position = newSizePos.Item2;
    }
}
