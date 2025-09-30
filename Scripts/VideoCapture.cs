using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class VisualizerRecorder : MonoBehaviour
{
    public Camera captureCamera;
    public int width = 1920;
    public int height = 1080;
    public int frameRate = 60;
    public float speedupFactor = 4f;

    private string framesPath;
    private string outputPath;
    private string currentPath;
    private int frameCount;

    public event Action<string> OnRecordingFinished;

    public void StartRecording(VisualizerProject project)
    {
        StartCoroutine(CaptureVisualizerProject(project));
    }

    private IEnumerator CaptureVisualizerProject(VisualizerProject project)
    {
        string projectName = project.project;

        string basePath = Path.Combine(Application.dataPath, "Videos", projectName);
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
                var audioClip = DownloadHandlerAudioClip.GetContent(www); float duration = audioClip.length; // total video duration based on audio

                Time.captureFramerate = frameRate;
                Time.timeScale = speedupFactor;
                frameCount = 0;

                int totalFrames = Mathf.CeilToInt(duration * frameRate);

                UnityEngine.Debug.Log($"Starting capture: {totalFrames} frames for project {projectName}");

                for (int i = 0; i < totalFrames; i++)
                {
                    yield return new WaitForEndOfFrame();

                    float currentAudioTime = frameCount / (float)frameRate;

                    // --- Apply visual effects based on project ---
                    // Set background, logo, effects, partials, lyrics, screen shake
                    // Example:
                    // UpdateBackground(project, currentAudioTime);
                    // UpdateLogo(project, currentAudioTime);
                    // UpdateLyrics(project.lyrics, currentAudioTime);
                    // ApplyScreenShake(project, currentAudioTime);
                    // UpdateAudioPartials(project, currentAudioTime);

                    // Capture frame
                    RenderTexture rt = new RenderTexture(width, height, 24);
                    captureCamera.targetTexture = rt;
                    Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                    captureCamera.Render();
                    RenderTexture.active = rt;
                    tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    tex.Apply();

                    string frameFile = Path.Combine(framesPath, $"frame_{frameCount:D06}.png");
                    File.WriteAllBytes(frameFile, tex.EncodeToPNG());

                    captureCamera.targetTexture = null;
                    RenderTexture.active = null;
                    Destroy(rt);
                    Destroy(tex);

                    frameCount++;
                }

                Time.captureFramerate = 0;
                Time.timeScale = 1f;

                UnityEngine.Debug.Log("Capture finished. Starting FFmpeg...");
                StartCoroutine(RunFFmpeg(projectName));
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

    private IEnumerator RunFFmpeg(string projectName)
    {
        yield return null;

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string outputFile = Path.Combine(outputPath, $"{projectName}_{timestamp}.mp4");
        string currentFile = Path.Combine(currentPath, $"{projectName}.mp4");

        string args = $"-y -framerate {frameRate} -i \"{framesPath}/frame_%06d.png\" -c:v libx264 -pix_fmt yuv420p -r {frameRate} \"{outputFile}\"";

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
            File.Copy(outputFile, currentFile, true);
            OnRecordingFinished?.Invoke(outputFile);
        };

        ffmpeg.Start();
        ffmpeg.BeginOutputReadLine();
        ffmpeg.BeginErrorReadLine();
    }
}
