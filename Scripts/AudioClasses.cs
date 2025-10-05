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
}

public enum Menus
{
    Options,
    LoadProject,
    Lyrics
}