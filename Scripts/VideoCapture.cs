using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VideoCapture : MonoBehaviour
{
    [HideInInspector]
    public CaptureManager captureManager;
    AudioManager audioManager;
    public Camera captureCamera;
    public int width = 1920;
    public int height = 1080;
    public int frameRate = 60;

    // Crop region (pixel coordinates relative to source frames)
    // Adjust these for your display area:
    int cropX = 0;
    int cropY = 61;
    int cropWidth = 1805;
    int cropHeight = 1018;
    private string framesPath;
    private string outputPath;
    private string currentPath;
    private int frameCount;

    public event Action<string> OnRecordingFinished;

    public string projectNameOut = "";
    public float currentTime = 0;
    public float totalLength = -1;
    public RawImage backgroundImageRI;
    public Transform lyricHolder;
    public Transform texturesHolder;

    void Start()
    {
        audioManager = AudioManager.instance;
    }

    public void StartRecording(VisualizerProject new_project)
    {
        StartCoroutine(CaptureVisualizerProject(new_project));
    }

    private IEnumerator CaptureVisualizerProject(VisualizerProject project)
    {
        string projectName = project.project;
        projectNameOut = projectName;

        string basePath = Path.Combine(Application.dataPath, "../", "Videos", projectName);
        framesPath = Path.Combine(basePath, "Frames");
        outputPath = Path.Combine(basePath, "Outputs");
        currentPath = Path.Combine(basePath, "Current");

        Directory.CreateDirectory(framesPath);
        Directory.CreateDirectory(outputPath);
        Directory.CreateDirectory(currentPath);

        // Clear Frames and Current
        if (Directory.Exists(framesPath)) Directory.Delete(framesPath, true);
        Directory.CreateDirectory(framesPath);
        if (Directory.Exists(currentPath)) Directory.Delete(currentPath, true);
        Directory.CreateDirectory(currentPath);

        // Load audio clipstring url = "file://" + path;
        string audioPath = project.fullPath;
        string url = "file://" + audioPath;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, GetAudioType(audioPath)))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                //Audio failed to load kill process
            }
            else
            {
                var audioClip = DownloadHandlerAudioClip.GetContent(www);
                float duration = audioClip.length; // total video duration based on audio
                totalLength = audioClip.length;

                int totalFrames = Mathf.CeilToInt(duration * frameRate);

                UnityEngine.Debug.Log($"Starting capture: {totalFrames} frames for project {projectName}");

                //Load in Background
                string path = project.backgroundPath;
                if (path != "")
                {
                    string background_url = "file://" + path;
                    using (UnityWebRequest background_www = UnityWebRequestTexture.GetTexture(background_url))
                    {
                        yield return background_www.SendWebRequest();

                        if (background_www.result != UnityWebRequest.Result.Success)
                        {
                        }
                        else
                        {
                            Texture2D tex = DownloadHandlerTexture.GetContent(background_www);
                            backgroundImageRI.texture = tex;
                        }
                    }
                }

                yield return new WaitForEndOfFrame();


                Time.captureFramerate = frameRate;
                frameCount = 0;
                for (int i = 0; i < totalFrames; i++)
                {
                    float currentAudioTime = frameCount / (float)frameRate;
                    currentTime = currentAudioTime;

                    // --- Apply visual effects based on project ---
                    // UpdateIcons(project);
                    #region UpdateLyrics
                    //Get all Lyrics that should be on display
                    List<LyricLine> currentLyricsLines = project.lyrics.FindAll(x => x.WithinTime(currentTime));

                    //Display spawn any that isnt already
                    foreach (LyricLine l in currentLyricsLines)
                    {
                        bool exists = false;

                        for (int il = lyricHolder.childCount - 1; il >= 0; il--)
                        {
                            Transform ch = lyricHolder.GetChild(il);

                            if (ch != null)
                            {
                                if (ch.gameObject.TryGetComponent<WordManager>(out var cL))
                                {
                                    if (cL.lyricLine == l)
                                    {
                                        cL.capture = this;
                                        exists = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!exists)
                        {
                            GameObject go = Instantiate(audioManager.wordDisplay, lyricHolder);

                            WordManager wm = go.GetComponent<WordManager>();
                            if (wm != null)
                            {
                                wm.lyricLine = l;
                                wm.capture = this;
                            }
                        }
                    }
                    #endregion
                    // ApplyScreenShake(project, currentAudioTime);
                    // UpdateAudioPartials(project, currentAudioTime);


                    yield return new WaitForEndOfFrame();
                    // Capture frame
                    RenderTexture rt = new RenderTexture(width, height, 24);
                    captureCamera.targetTexture = rt;
                    Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                    captureCamera.Render();
                    RenderTexture.active = rt;
                    tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    tex.Apply();

                    // ✅ Crop texture in Unity (bottom-left origin)
                    Color[] croppedPixels = tex.GetPixels(cropX, cropY, cropWidth, cropHeight);
                    Texture2D croppedTex = new Texture2D(cropWidth, cropHeight, TextureFormat.RGB24, false);
                    croppedTex.SetPixels(croppedPixels);
                    croppedTex.Apply();

                    // ✅ Scale cropped texture to 1920x1080 using RenderTexture (reliable scaling)
                    RenderTexture scaleRT = new RenderTexture(1920, 1080, 0);
                    Graphics.Blit(croppedTex, scaleRT);

                    RenderTexture.active = scaleRT;
                    Texture2D finalTex = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
                    finalTex.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
                    finalTex.Apply();

                    // ✅ Save final scaled frame
                    string frameFile = Path.Combine(framesPath, $"frame_{frameCount:D06}.png");
                    File.WriteAllBytes(frameFile, finalTex.EncodeToPNG());

                    // Cleanup
                    captureCamera.targetTexture = null;
                    RenderTexture.active = null;
                    Destroy(rt);
                    Destroy(tex);
                    Destroy(croppedTex);
                    Destroy(finalTex);
                    Destroy(scaleRT);

                    frameCount++;
                    yield return new WaitForEndOfFrame();
                }

                Time.captureFramerate = 0;

                UnityEngine.Debug.Log("Capture finished. Starting FFmpeg...");
                StartCoroutine(RunFFmpeg(projectName,audioPath));
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
            case ".mp4": return AudioType.MPEG;
            default: return AudioType.UNKNOWN;
        }
    }

    private IEnumerator RunFFmpeg(string projectName, string audioPath)
    {
        yield return null;

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string outputFile = Path.Combine(outputPath, $"{projectName}_{timestamp}.mp4");
        string currentFile = Path.Combine(currentPath, $"{projectName}.mp4");

        // ffmpeg command:
        //  -framerate: input frame rate (images)
        //  -i: path to frames
        //  -i: path to audio file
        //  -c:v libx264: encode video using H.264
        //  -c:a aac: encode audio using AAC
        //  -b:a 320k: set audio bitrate
        //  -pix_fmt yuv420p: ensures wide compatibility
        //  -shortest: trims video/audio to shortest input (important)
        //  -r: final video frame rate
        string args =
            $"-y -framerate {frameRate} -i \"{framesPath}/frame_%06d.png\" " +
            $"-i \"{audioPath}\" " +
            $"-filter:v \"scale=1920:1080\" " +
            $"-c:v libx264 -pix_fmt yuv420p -r {frameRate} " +
            $"-c:a aac -b:a 320k -shortest \"{outputFile}\"";

        Process ffmpeg = new Process();
        ffmpeg.StartInfo.FileName = @"C:\ffmpeg\bin\ffmpeg.exe";
        ffmpeg.StartInfo.Arguments = args;
        ffmpeg.StartInfo.UseShellExecute = false;
        ffmpeg.StartInfo.RedirectStandardOutput = true;
        ffmpeg.StartInfo.RedirectStandardError = true;
        ffmpeg.StartInfo.CreateNoWindow = true;
        ffmpeg.EnableRaisingEvents = true;
        ffmpeg.Exited += (sender, e) =>
        {
            try
            {
                File.Copy(outputFile, currentFile, true);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to copy video: {ex.Message}");
            }

            OnRecordingFinished?.Invoke(outputFile);
            if (captureManager != null)
            {
                // Notify main UI that render finished, e.g.
                captureManager.OnRecordingFinished(projectName, outputFile);
            }
        };

        ffmpeg.Start();
        ffmpeg.BeginOutputReadLine();
        ffmpeg.BeginErrorReadLine();
        Destroy(gameObject);
    }
}
