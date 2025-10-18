using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    public VideoCapture capture;

    [HideInInspector]
    public float current = 0;

    [HideInInspector]
    public float total = -1;

    public void Start()
    {
        capture.captureManager = this;
    }

    public void Update()
    {
        current = capture.currentTime;
        total = capture.totalLength;
    }

    public void StartRecording(VisualizerProject new_project)
    {
        capture.StartRecording(new_project);
    }

    public void OnRecordingFinished(string projectName, string outputFile)
    {
        Debug.Log("Completed exporting (" + projectName + ")");
        Debug.Log("Output Path (" + outputFile + ")");

        Invoke(nameof(DestroyThis), 5);

        // Open folder in File Explorer
        try
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + outputFile + "\"");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Failed to open file in Explorer: " + ex.Message);
            Application.OpenURL(System.IO.Path.GetDirectoryName(outputFile));
        }
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
