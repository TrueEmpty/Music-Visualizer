using UnityEngine;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    RectTransform rectTransform;
    RawImage image;
    Outline outline;
    Shadow shadow;

    AudioManager audioManager;
    public TexturePrint texturePrint;
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
        float sT = texturePrint.time;
        float eT = texturePrint.time + texturePrint.length;

        float curTime = audioManager.audioSource.time;
        if (sT <= curTime && curTime <= eT)
        {
            if (texturePrint.image != null && image.texture == null)
            {
                image.texture = texturePrint.image;
            }

            Selecting();
        }
        else
        {
            Destroy(gameObject);
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
            if (Input.GetMouseButtonDown(0) && dragWindow.isMouseOver)
            {
                startingSelect = true;
            }

            if (Input.GetMouseButtonUp(0) && startingSelect)
            {
                if (dragWindow.isMouseOver)
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
    }
}
