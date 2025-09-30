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
    public LyricLine[] lyrics;
}

[System.Serializable]
public class LyricLine
{
    public float time;
    public string text;
}

public enum Menus
{
    Options,
    LoadProject,
    Lyrics
}