using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VisualizerProject
{
    public string project = "";
    public string audioName = "";
    public string fullPath = "";
    public string audioPath;
    public string backgroundPath = "";
    public string logoPath;
    public string backgroundEffect;
    public string title;
    public List<LyricLine> lyrics = new List<LyricLine>();

    public List<TexturePrint> textures = new List<TexturePrint>();
}

[System.Serializable]
public class LyricLine
{
    public float time; //Time to start
    public float length; //In seconds
    public string text;

    public Vector2 position; //X and Y position from bottom left based on a 802.56 , 451.44 background
    public Vector2 size = new Vector2(100,50); //Width and Height based on a 802.56 , 451.44 background
    public int order; //What order to draw in

    public LyricLine()
    {

    }

    public LyricLine(float time, int index, Vector2 position, Vector2 size)
    {
        this.time = time;
        this.length = 1000000f;
        this.text = "Auto line #" + index;
        this.position = position;
        this.size = size;
        this.order = 0;
    }

    public bool WithinTime(float currentTime)
    {
        return (time <= currentTime && currentTime <= (time + length));
    }
}

[System.Serializable]
public class TexturePrint
{
    public float time; //Time to start
    public float length; //In seconds
    public string texture;
    public Texture image;

    public Vector2 position; //X and Y position from bottom left based on a 802.56 , 451.44 background
    public Vector2 size = new Vector2(100, 100); //Width and Height based on a 802.56 , 451.44 background
    public int order; //What order to draw in

    public TexturePrint()
    {

    }

    public TexturePrint(string path)
    {
        time = -1000;
        length = 100000;
        texture = path;
        position = Vector2.zero;
    }

    public bool WithinTime(float currentTime)
    {
        return (time <= currentTime && currentTime <= (time + length));
    }
}

public enum Menus
{
    Options,
    LoadProject,
    Lyrics
}