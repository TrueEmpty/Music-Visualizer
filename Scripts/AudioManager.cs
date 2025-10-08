using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    FileBrowser fileBrowser;
    public string lastPath;
    AudioClip audioClip;
    public AudioSource audioSource;
    public InputField projectName;
    public Text currentAudio;
    public Text currentAudioLength;
    public Text maxAudioLength;

    float audioLength = 0;
    public Texture playTexture;
    public Texture pauseTexture;
    public bool paused = false;

    [Header("Buttons")]
    public RawImage playButton;
    public Slider audioSlider;

    [SerializeField]
    List<VisualizerProject> projects = new List<VisualizerProject>();
    public int currentProject = -1;

    [Header("Bckground and Icon")]
    public MouseOverRectTransformPosition display;
    Texture backgroundImage;
    public RawImage backgroundImageRI;
    public InputField titleName;
    public Text titleText;
    public Image playHandle;
    public Image playFillBar;

    [Header("Menus")]
    public Menus currentMenu = Menus.Options;
    public GameObject mainMenu;
    public GameObject loadProjectMenu;
    public GameObject loadLyricsMenu;

    [Header("Lyric Setup Info")]
    public GameObject indexTimeObj;
    public Transform indexTimeHolder;
    public RectTransform contentLyricHolder;
    public GameObject wordDisplay;
    public InputField bulkLyrics;
    public Transform lyricHolder;
    public WordManager currentAutoLyric = null;
    public bool autoLyricAddMode = false;

    [Header("Icon Setup Info")]
    public Transform texturesHolder;
    public GameObject iconDisplay;

    public GameObject selectedObject = null;


    [Header("Color Setup Info")]
    public Color borderColor;
    public Color autoLyricColor;
    public Color sliderColor;


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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fileBrowser = FileBrowser.instance;
        audioSource = GetComponent<AudioSource>();
        LoadProjects();
    }

    // Update is called once per frame
    void Update()
    {
        ShowMenus();

        switch (currentMenu)
        {
            case Menus.Options:
                OptionDisplay();
                ExtraButtons();
            break;
            case Menus.LoadProject:

            break;
            case Menus.Lyrics:

            break;
        }

        ShowDisplay();
        SelectObject();
        LyricUpdate();
    }

    void LyricUpdate()
    {
        if (CurrentProject(out VisualizerProject cP))
        {
            //Get all Lyrics that should be on display
            List<LyricLine> currentLyricsLines = cP.lyrics.FindAll(x=> x.WithinTime(audioSource.time));

            //Display spawn any that isnt already
            foreach(LyricLine l in currentLyricsLines)
            {
                bool exists = false;

                for(int i = lyricHolder.childCount - 1; i >= 0; i--)
                {
                    Transform ch = lyricHolder.GetChild(i);

                    if (ch != null)
                    {                            
                        if (ch.gameObject.TryGetComponent<WordManager>(out var cL))
                        {
                            if (cL.lyricLine == l)
                            {
                                exists = true;
                                break;
                            }
                        }
                    }
                }

                if(!exists)
                {
                    LoadWordDisplay(l);
                }
            }
        }
    }

    void SelectObject()
    {
        if(selectedObject != null)
        {
            if(Input.GetMouseButtonUp(1) && !autoLyricAddMode)
            {
                selectedObject = null;
            }
        }
    }

    void ShowMenus()
    {
        mainMenu.SetActive(currentMenu == Menus.Options);
        loadProjectMenu.SetActive(currentMenu == Menus.LoadProject);
        loadLyricsMenu.SetActive(currentMenu == Menus.Lyrics);

        if (currentMenu == Menus.Lyrics)
        {
            UpdateLyricTab();
        }
    }

    //Display Option Info
    void OptionDisplay()
    {   
        if (!CurrentProject(out VisualizerProject cP))
        {
            cP = new VisualizerProject();
        }

        currentAudio.text = "Audio Track: ";
        currentAudio.text += (cP.audioName == "") ? "None" : cP.audioName;

        playButton.texture = (audioSource.isPlaying) ? pauseTexture : playTexture;

        TimeSpan time = TimeSpan.FromSeconds(audioSource.time);
        currentAudioLength.text = (time.TotalHours > 0) ? time.ToString(@"hh\:mm\:ss\:fff") : time.ToString(@"mm\:ss\:fff");

        time = TimeSpan.FromSeconds(audioLength);
        maxAudioLength.text = (time.TotalHours > 0) ? time.ToString(@"hh\:mm\:ss\:fff") : time.ToString(@"mm\:ss\:fff");

        if(audioSource.isPlaying)
        {
            audioSlider.value = audioSource.time;
        }
    }

    void ExtraButtons()
    {
        if(!InputFieldActive())
        {
            if (currentMenu == Menus.Options)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TogglePlay();
                }

                //Skim Right
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    float multiplier = (audioSource.isPlaying) ? 100 : 1;

                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        multiplier *= .05f;
                    }

                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        multiplier *= 3f;
                    }

                    audioSlider.value += .01f * multiplier * Time.deltaTime;
                    audioSlider.value = Mathf.Clamp(audioSlider.value, 0f, audioSlider.maxValue);
                    MoveTrack();
                }

                //Skim Left
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    float multiplier = (audioSource.isPlaying) ? 200 : 1;

                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        multiplier *= .25f;
                    }

                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        multiplier *= 3f;
                    }

                    audioSlider.value -= .01f * multiplier * Time.deltaTime;
                    audioSlider.value = Mathf.Clamp(audioSlider.value, 0f, audioSlider.maxValue);
                    MoveTrack();
                }
            }

            //Auto Lyric Mode
            if (CurrentProject(out VisualizerProject cP))
            {
                autoLyricAddMode = (Input.GetKey(KeyCode.L));
                if (autoLyricAddMode)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (currentAutoLyric == null)
                        {
                            //Create new lyric line
                            display.GetMousePosition(out Vector2 mousePoint);
                            cP.lyrics.Add(new LyricLine(audioSource.time,cP.lyrics.Count,
                                mousePoint,new Vector2(300,50)));

                            //Create Lyric Display with default text
                            GameObject go = Instantiate(wordDisplay, lyricHolder);

                            currentAutoLyric = go.GetComponent<WordManager>();
                            if (currentAutoLyric != null)
                            {
                                currentAutoLyric.lyricLine = cP.lyrics[^1];
                            }
                        }
                    }

                    if (Input.GetMouseButtonUp(1))
                    {
                        if (currentAutoLyric != null)
                        {
                            //Finish new lyric line
                            float length = audioSource.time - currentAutoLyric.lyricLine.time;
                            currentAutoLyric.lyricLine.length = length;

                            currentAutoLyric = null;
                        }
                    }
                }
                else
                {
                    if (currentAutoLyric != null)
                    {
                        //Finish new lyric line
                        float length = audioSource.time - currentAutoLyric.lyricLine.time;
                        currentAutoLyric.lyricLine.length = length;
                    }

                    currentAutoLyric = null;
                }
            }
            else
            {
                autoLyricAddMode = false;
            }
        }
    }

    void ShowDisplay()
    {
        if (!CurrentProject(out VisualizerProject cP))
        {
            cP = new VisualizerProject();
        }

        backgroundImageRI.gameObject.SetActive(backgroundImage != null);
        if (backgroundImage != null)
        {
            backgroundImageRI.texture = backgroundImage;
        }

        titleText.text = cP.title;

        playHandle.color = (autoLyricAddMode) ? autoLyricColor : sliderColor;
        playFillBar.color = (autoLyricAddMode) ? autoLyricColor : sliderColor;
    }

    public bool CurrentProject(out VisualizerProject cP)
    {
        cP = (currentProject >= 0 && currentProject < projects.Count) ? projects[currentProject] : null;
        return (cP != null);
    }

    public void ChangeProjectName()
    {
        if (CurrentProject(out VisualizerProject cP))
        {
            cP.project = projectName.text;
        }
    }

    public void ChangeBulkLyrics()
    {
        if (CurrentProject(out VisualizerProject cP))
        {

        }
    }

    public void ChangeTitleName()
    {
        if (CurrentProject(out VisualizerProject cP))
        {
            cP.title = titleName.text;
        }
    }

    bool InputFieldActive()
    {
        var go = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        return go != null && go.GetComponent<InputField>() != null;
    }

    void LyricTabSetup()
    {
        //Clear Index Time and Dashes
        for(int i = indexTimeHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(indexTimeHolder.GetChild(i).gameObject);
        }

        //Set up Index Time and Dashes
        //based on total number or seconds with a Short Dash for each 20 milli-seconds
        //(5 total first being triple) with triple dashes on each second
        int tS = Mathf.CeilToInt(audioLength);

        for (int i = 0; i <= tS; i++)
        {
            TimeSpan cTime = TimeSpan.FromSeconds(i);

            //Create Seconds Index
            GameObject iTime = Instantiate(indexTimeObj,indexTimeHolder) as GameObject;

            Text iText = iTime.GetComponent<Text>();

            if(iText != null)
            {
                iText.text = cTime.ToString(@"hh\:mm\:ss") + "---";
            }

            if (i < tS)
            {
                //Create Milli-Second Dashes
                for(int r = 0; r < 4; r++)
                {
                    GameObject dTime = Instantiate(indexTimeObj, indexTimeHolder) as GameObject;

                    Text dText = dTime.GetComponent<Text>();

                    if (dText != null)
                    {
                        dText.text = "--";
                    }
                }
            }
        }
    }

    void UpdateLyricTab()
    {
        CurrentProject(out VisualizerProject cP);

        //Adjust content size
        float iTHRT = indexTimeHolder.GetComponent<RectTransform>().sizeDelta.y;
        if(iTHRT <= 0)
        {
            //Add a button to add audio
            contentLyricHolder.sizeDelta = new Vector2(contentLyricHolder.sizeDelta.x, 50);

        }
        else
        {
            contentLyricHolder.sizeDelta = new Vector2(contentLyricHolder.sizeDelta.x, iTHRT + 10);
        }

        if (cP != null)
        {
        }
    }

    public void LoadWordDisplay(LyricLine lL)
    {
        GameObject go = Instantiate(wordDisplay, lyricHolder);

        WordManager wm = go.GetComponent<WordManager>();
        if (wm != null)
        {
            wm.lyricLine = lL;
        }
    }

    #region Audio Source Actions
    public void OpenAudio()
    {
        string aPath = fileBrowser.OpenFile(lastPath);

        if (aPath != null)
        {
            StartCoroutine(LoadAudio(aPath));
        }
    }

    private IEnumerator LoadAudio(string path, bool autoStartPlay = true)
    {
        yield return new WaitForEndOfFrame();

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
                int lastSlash = path.LastIndexOf('\\');
                string pathPlaying = path[..lastSlash];
                string fileName = path.Substring(lastSlash + 1);

                LoadInAudioData(path, pathPlaying, fileName, DownloadHandlerAudioClip.GetContent(www),autoStartPlay);
                LyricTabSetup();
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

    public void LoadInAudioData(string path, string pathPlaying, string fileName, AudioClip clip, bool autoStartPlay)
    {
        if (!CurrentProject(out VisualizerProject ncP))
        {
            projects.Add(new VisualizerProject());
            currentProject = projects.Count - 1;
        }

        CurrentProject(out VisualizerProject cP);
        cP.fullPath = path;
        cP.audioPath = pathPlaying;
        cP.audioName = fileName;
        Play(clip);

        if(!autoStartPlay)
        {
            TogglePlay();
        }
    }

    public void Play(AudioClip clip = null)
    {
        CurrentProject(out VisualizerProject cP);

        if (clip != null)
        {
            audioClip = clip;
        }

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioLength = audioClip.length;
            audioSlider.maxValue = audioClip.length;
            paused = false;
            audioSource.Play();
            Debug.Log("Playing: " + cP.audioName);
        }
        else
        {
            Debug.LogError("No Audio Clip to play!");
        }
    }

    public void TogglePlay()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            paused = true;
        }
        else if(paused)
        {
            audioSource.UnPause();
            paused = false;
        }
        else //It ended so play from where it left off
        {
            audioSource.Play();
            audioSource.time = audioSlider.value;
            paused = true;
        }
    }

    public void MoveTrack()
    {
        audioSource.time = audioSlider.value;
    }
    #endregion

    #region Background Actions
    public void OpenBackground()
    {
        string aPath = fileBrowser.OpenFile(lastPath, ".png", ".jpg", ".jpeg");

        if (!string.IsNullOrEmpty(aPath))
        {
            StartCoroutine(LoadBackground(aPath));
        }
    }

    private IEnumerator LoadBackground(string path)
    {
        yield return new WaitForEndOfFrame();

        string url = "file://" + path;
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Texture load failed: " + www.error);
            }
            else
            {
                int lastSlash = path.LastIndexOf('\\');
                string pathPlaying = path[..lastSlash];
                string fileName = path.Substring(lastSlash + 1);

                Texture2D tex = DownloadHandlerTexture.GetContent(www);
                LoadInBackgroundData(path, tex);
            }
        }
    }

    public void LoadInBackgroundData(string path, Texture texture)
    {
        if (!CurrentProject(out VisualizerProject ncP))
        {
            projects.Add(new VisualizerProject());
            currentProject = projects.Count - 1;
        }

        CurrentProject(out VisualizerProject cP);
        cP.backgroundPath = path;

        backgroundImage = texture;
        Debug.Log("Background set: " + path);
    }
    #endregion

    #region Icon Actions
    public void OpenIcon()
    {
        string aPath = fileBrowser.OpenFile(lastPath, ".png", ".jpg", ".jpeg");

        if (!string.IsNullOrEmpty(aPath))
        {
            TexturePrint nTP = LoadInIconData(aPath);
            StartCoroutine(LoadIcon(nTP));
        }
    }

    private IEnumerator LoadIcon(TexturePrint tP)
    {
        yield return new WaitForEndOfFrame();

        string path = tP.texture;
        string url = "file://" + path;
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Texture load failed: " + www.error);
            }
            else
            {
                int lastSlash = path.LastIndexOf('\\');
                string pathPlaying = path[..lastSlash];
                string fileName = path.Substring(lastSlash + 1);

                Texture2D tex = DownloadHandlerTexture.GetContent(www);
                tP.image = tex;

                CreateIcon(tP);
            }
        }
    }

    void CreateIcon(TexturePrint tP)
    {
        GameObject go = Instantiate(iconDisplay, texturesHolder);

        IconManager im = go.GetComponent<IconManager>();
        if (im != null)
        {
            im.texturePrint = tP;
        }
    }

    public TexturePrint LoadInIconData(string path)
    {
        if (!CurrentProject(out VisualizerProject ncP))
        {
            projects.Add(new VisualizerProject());
            currentProject = projects.Count - 1;
        }

        CurrentProject(out VisualizerProject cP);
        TexturePrint result = new TexturePrint(path);
        cP.textures.Add(result);
        return result;
    }
    #endregion

    private void OnApplicationQuit()
    {
        SaveProjects();
    }

    void SaveProjects()
    {
        PlayerPrefs.SetInt("MaxProjects",projects.Count);
        PlayerPrefs.SetInt("CurrentProject",currentProject);

        for (int i = 0; i < projects.Count; i++)
        {
            PlayerPrefs.SetString("Project " + i, JsonUtility.ToJson(projects[i]));
        }

        PlayerPrefs.Save();
    }

    void LoadProjects()
    {
        int pC = PlayerPrefs.GetInt("MaxProjects");
        currentProject = PlayerPrefs.GetInt("CurrentProject");

        for (int i = 0; i < pC; i++)
        {
            string lP = PlayerPrefs.GetString("Project " + i);

            projects.Add((VisualizerProject)JsonUtility.FromJson(lP,typeof(VisualizerProject)));
        }

        if(CurrentProject(out VisualizerProject cP))
        {
            if (cP.project != null)
            {
                projectName.text = cP.project;
            }

            if (cP.title != null)
            {
                titleName.text = cP.title;
            }

            if (cP.lyrics.Count > 0)
            {
                for (int i = 0; i < cP.lyrics.Count; i++)
                {
                    //Create a word Display
                }
            }

            if (cP.textures.Count > 0)
            {
                for (int i = 0; i < cP.textures.Count; i++)
                {
                    StartCoroutine(LoadIcon(cP.textures[i]));
                }
            }

            if (cP.fullPath != null)
            {
                if(cP.fullPath != "")
                {
                    StartCoroutine(LoadAudio(cP.fullPath, false));
                }
                else
                {
                    Debug.Log("Full Path blank!\n" + cP.fullPath);
                }
            }

            if (cP.backgroundPath != null)
            {
                if (cP.backgroundPath != "")
                {
                    StartCoroutine(LoadBackground(cP.backgroundPath));
                }
                else
                {
                    Debug.Log("Background Path blank!\n" + cP.backgroundPath);
                }
            }
        }
    }

    public void NewProject()
    {
        projects.Add(new VisualizerProject());
        currentProject = projects.Count - 1;
        audioLength = 0;
        audioClip = null;
        projectName.text = "";
        backgroundImageRI.texture = null;
        backgroundImage = null;
    }

    public void ChangeProject(int changeToIndex)
    {
        Menus changeTo = Menus.Options;
        switch (changeToIndex)
        {
            case 0:
                changeTo = Menus.Options;
            break;
            case 1:
                changeTo = Menus.LoadProject;
            break;
            case 2:
                changeTo = Menus.Lyrics;
            break;
            default:
                changeTo = Menus.Options;
            break;
        }

        switch (currentMenu)
        {
            case Menus.Options:
                //Change Options Menu to Load Project Menu
                currentMenu = changeTo;
                break;
            case Menus.LoadProject:
                //Close Load Project Menu
                if (changeTo == currentMenu)
                {
                    currentMenu = Menus.Options;
                }
                else
                {
                    currentMenu = changeTo;
                }
            break;
            case Menus.Lyrics:
                //Close Load Project Menu
                if (changeTo == currentMenu)
                {
                    currentMenu = Menus.Options;
                }
                else
                {
                    currentMenu = changeTo;
                }
            break;
        }
    }
}
