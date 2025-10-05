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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = AudioManager.instance;    
        
        rectTransform = GetComponent<RectTransform>();
        word = GetComponent<Text>();
        outline = GetComponent<Outline>();
        shadow = GetComponent<Shadow>();

        word.text = lyricLine.text;
    }

    // Update is called once per frame
    void Update()
    {
        float sT = lyricLine.time;
        float eT = lyricLine.time + lyricLine.length;

        float curTime = audioManager.audioSource.time;
        if(sT <= curTime && curTime <= eT)
        {

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
