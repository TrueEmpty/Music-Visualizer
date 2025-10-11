using UnityEngine;
using UnityEngine.UI;

public class LyricInfo : MonoBehaviour
{
    AudioManager audioManager;
    RectTransform rt;
    public LyricLine lyricLine;
    InputField inputField;

    float secondLength = 15.5f;
    float secondDistance = -12.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = AudioManager.instance;
        rt = GetComponent<RectTransform>();
        inputField = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioManager.CurrentProject(out VisualizerProject cP))
        {
            if (lyricLine != null)
            {
                if(cP.lyrics.Contains(lyricLine))
                {
                    //Size and Position
                    float height = lyricLine.length * secondLength;
                    rt.sizeDelta = new Vector2 (rt.sizeDelta.x, height);

                    float distance = lyricLine.time * secondDistance;
                    rt.anchoredPosition = new Vector2(0, distance);

                    //Update Text
                    if(!audioManager.InputFieldActive())
                    {
                        inputField.text = lyricLine.text;
                    }
                }
                else
                {
                    DestroySelf();
                }
            }
            else
            {
                DestroySelf();
            }
        }
        else
        {
            DestroySelf();
        }
    }

    public void ChangeLyrics()
    {
        lyricLine.text = inputField.text;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
