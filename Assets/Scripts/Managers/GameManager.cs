using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json.Linq;

public class GameManager : MonoBehaviour
{
    public bool adminMode;

    [HideInInspector]
    public static GameManager instance;
    [HideInInspector]
    public static float fadeTime = 1f;
    [HideInInspector]
    public bool lastLevelWasStory;
    [HideInInspector]
    public int lastLevelPlayedID;
    [HideInInspector]
    public int deathCounter;
    [HideInInspector]
    public int collectablesTotal;

    public List<JObject> storyModeLevelList;
    [HideInInspector]
    public bool isAbleToPlay;
    [HideInInspector]
    public Material lastFadedMaterial;

    private JObject _levelData;
    private bool _playingLevel;
    private bool _levelMakerOn;
    private bool _editingLevel; // Quizás se use esto en vez del "needsToBeCreated"...
    private Checkpoint _lastCheckpoint;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
            instance = this;
    }

    void Update() // Después pasar todo esto a un Input Manager...
    {
        if (IsUsingLevelMaker() && Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(ExecuteQuitLevel());
    }

    IEnumerator ExecuteQuitLevel()
    {
        CanvasUIManager.instance.EnableFade(CanvasUIManager.instance.diagonalLinesFade);

        yield return new WaitForSeconds(GameManager.fadeTime);

        QuitLevel();
    }

    public void QuitLevel()
    {
        deathCounter = 0;
        SceneManager.LoadScene("Menu");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            _levelData = null;
            _playingLevel = false;
            _levelMakerOn = false;
        }
    }

    public void PlayingLevel(bool value)
    {
        _playingLevel = value;
    }

    public void UsingLevelMaker(bool value)
    {
        _levelMakerOn = value;
    }

    public void EditingLevel(bool value)
    {
        _editingLevel = value;
    }

    public bool IsPlayingLevel()
    {
        return _playingLevel;
    }

    public bool IsUsingLevelMaker()
    {
        return _levelMakerOn;
    }

    public bool IsEditingLevel()
    {
        return _levelMakerOn;
    }

    public IEnumerator PlayNextLevel(float time)
    {
        deathCounter = 0;
        CanvasUIManager.instance.youWinScreen.SetActive(true);
        CanvasUIManager.instance.grabObjectAlert.SetActive(false);
        CanvasUIManager.instance.deaths.SetActive(false);
        CanvasUIManager.instance.collectables.SetActive(false);

        yield return new WaitForSeconds(time);

        if (lastLevelWasStory)
        {
            if (lastLevelPlayedID < storyModeLevelList.Count)
                LevelListManager.PlayNextLevel(storyModeLevelList[lastLevelPlayedID]);
            else
                QuitLevel();
        }
        else
        {
            QuitLevel();
            StartCoroutine(GoToCustomLevelsSubMenu());
        }
    }

    IEnumerator GoToStoryModeSubMenu()
    {
        yield return new WaitForSeconds(0.05f);
        MenuManager.instance.OpenPlaySubMenu();
        MenuManager.instance.OpenPlaySubMenuStoryMode();
    }

    IEnumerator GoToCustomLevelsSubMenu()
    {
        yield return new WaitForSeconds(0.05f);
        MenuManager.instance.OpenPlaySubMenu();
        MenuManager.instance.OpenPlaySubMenuCustomLevels();
    }

    public void UpdateCheckpoint(Checkpoint checkpoint)
    {
        var newCheckpointWasEnabled = checkpoint.Enable();
        if (newCheckpointWasEnabled)
        {
            if (_lastCheckpoint != null)
                _lastCheckpoint.Disable();
            _lastCheckpoint = checkpoint;
        }
    }

    public void RestartLevel()
    {
        if (_levelData != null)
            SceneManager.LoadScene("Play");
    }

    public void LoadLevelData(JObject levelData)
    {
        _levelData = levelData;
        lastLevelPlayedID = (int)_levelData["levelID"];
        collectablesTotal = (int)_levelData["collectablesTotal"];
    }

    public JObject GetLevelData()
    {
        return _levelData;
    }

    public void OnLevelBuildingFinished()
    {

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}