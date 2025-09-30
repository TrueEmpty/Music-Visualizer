using SFB;
using System.Collections.Generic;
using UnityEngine;

public class FileBrowser : MonoBehaviour
{
    public static FileBrowser instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(instance);
            }
        }
    }

    public string OpenFile(string startingPath,params string[] extentions)
    {
        List<string> results = RunOpen(startingPath,false,extentions);

        if(results.Count > 0)
        {
            return results[0];
        }

        return null;
    }

    public List<string> OpenFiles(string startingPath, params string[] extentions)
    {
        return RunOpen(startingPath, true, extentions);
    }

    List<string> RunOpen(string startingPath, bool multi, params string[] extentions)
    {
        List<string> result = new List<string>();

        List<ExtensionFilter> filters = new List<ExtensionFilter>();

        if (extentions.Length == 0)
        {
            filters.Add(new ExtensionFilter(".mp3"));
            filters.Add(new ExtensionFilter(".ogg"));
            filters.Add(new ExtensionFilter(".wav"));
            filters.Add(new ExtensionFilter(".mp4"));
        }
        else
        {
            for (int i = 0; i < extentions.Length; i++)
            {
                filters.Add(new ExtensionFilter(extentions[i]));
            }
        }

        ExtensionFilter[] useExtensions = filters.ToArray();

        var paths = StandaloneFileBrowser.OpenFilePanel("Select an Audio Track", startingPath, useExtensions, multi);

        if (paths.Length > 0)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                result.Add(paths[i]);
            }
        }

        return result;
    }
}
