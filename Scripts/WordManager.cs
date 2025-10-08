using UnityEngine;
using UnityEngine.UI;

public class WordManager : MonoBehaviour
{
    RectTransform rectTransform;
    Text word;
    Outline outline;
    Shadow shadow;

    AudioManager audioManager;
    public LyricLine lyricLine;
    DragWindow dragWindow;
    Border border;

    bool startingSelect = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = AudioManager.instance;    
        
        rectTransform = GetComponent<RectTransform>();
        dragWindow = GetComponent<DragWindow>();
        border = GetComponent<Border>();
        word = GetComponent<Text>();
        outline = GetComponent<Outline>();
        shadow = GetComponent<Shadow>();

        PositioningAndLyrics();
    }

    // Update is called once per frame
    void Update()
    {
        if(lyricLine.WithinTime(audioManager.audioSource.time))
        {
            Selecting();
            PositioningAndLyrics();
        }
        else
        {
            Destroy(gameObject);
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
            if (Input.GetMouseButtonDown(0) && dragWindow.isMouseOver)
            {
                startingSelect = true;
            }

            if(Input.GetMouseButtonUp(0) && startingSelect)
            {
                if(dragWindow.isMouseOver)
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
    }

    void PositioningAndLyrics()
    {
        word.text = lyricLine.text;

        rectTransform.anchoredPosition = lyricLine.position;
        rectTransform.sizeDelta = lyricLine.size;
    }
}
