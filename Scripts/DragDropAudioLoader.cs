using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class DragDropAudioLoader : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public AudioSource audioSource;

#if UNITY_EDITOR
    void OnGUI()
    {
        Event e = Event.current;

        if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (string path in DragAndDrop.paths)
                {
                    if (IsAudioFile(path))
                    {
                        StartCoroutine(LoadAudio(path));
                    }
                }
            }
        }
    }
#endif

    private bool IsAudioFile(string path)
    {
        string ext = Path.GetExtension(path).ToLower();
        return ext == ".mp3" || ext == ".wav" || ext == ".ogg";
    }

    private IEnumerator LoadAudio(string path)
    {
        string url = "file://" + path;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, GetAudioType(path)))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Audio load failed: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
                Debug.Log("Loaded and playing: " + path);
            }
        }
    }

    private AudioType GetAudioType(string path)
    {
        string ext = Path.GetExtension(path).ToLower();
        switch (ext)
        {
            case ".mp3": return AudioType.MPEG;
            case ".ogg": return AudioType.OGGVORBIS;
            case ".wav": return AudioType.WAV;
            default: return AudioType.UNKNOWN;
        }
    }
}
