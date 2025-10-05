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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = AudioManager.instance;    
        
        rectTransform = GetComponent<RectTransform>();
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
        Debug.Log(sT + "," + eT + ":" + curTime);
        if (sT <= curTime && curTime <= eT)
        {
            if (texturePrint.image != null && image.texture == null)
            {
                image.texture = texturePrint.image;
            }

        }
        else
        {
            Debug.Log("Destroying");
            Destroy(gameObject);
        }
    }
}
