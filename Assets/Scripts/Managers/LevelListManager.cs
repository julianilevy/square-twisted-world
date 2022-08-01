using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LevelListManager : MonoBehaviour
{
    public FadeTransition menuCamera;
    public Material fadeMaterial;
    public GameObject storyModeLevelListReference;
    public GameObject customLevelListReference;
    public GameObject lmStoryModeLevelListReference;
    public GameObject lmCustomLevelListReference;
    public Button levelButtonBase;

    [HideInInspector]
    public static LevelListManager instance;
    [HideInInspector]
    public List<GameObject> storyModeLevelList;
    [HideInInspector]
    public List<GameObject> customLevelList;
    [HideInInspector]
    public List<GameObject> lmStoryModeLevelList;
    [HideInInspector]
    public List<GameObject> lmCustomLevelList;

    void Awake()
    {
        if (instance == null)
            instance = this;
        CreateLevelsDirectory();
        storyModeLevelList = new List<GameObject>();
        customLevelList = new List<GameObject>();
        lmStoryModeLevelList = new List<GameObject>();
        lmCustomLevelList = new List<GameObject>();
    }

    void CreateLevelsDirectory()
    {
        var storyModeDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Square Twisted World/Levels/Story Mode/";
        if (!Directory.Exists(storyModeDataPath))
        {
            Directory.CreateDirectory(storyModeDataPath);
            var dataPath = Application.streamingAssetsPath + "/Levels/Story Mode/";
            var directoryInfo = new DirectoryInfo(dataPath);
            var fileInfo = directoryInfo.GetFiles("*.json");

            foreach (var file in fileInfo)
            {
                string tempPath = Path.Combine(storyModeDataPath, file.Name);
                file.CopyTo(tempPath, false);
            }
        }
        var customLevelsDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Square Twisted World/Levels/Custom/";
        if (!Directory.Exists(customLevelsDataPath))
            Directory.CreateDirectory(customLevelsDataPath);
    }

    public void CreateLevelList(string folder, GameObject levelListReference, List<GameObject> levelList, Action<JObject> function)
    {
        var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Square Twisted World/Levels/" + folder + "/";
        if (!Directory.Exists(dataPath))
            return;
        var directoryInfo = new DirectoryInfo(dataPath);
        var fileInfo = directoryInfo.GetFiles("*.json");
        GameManager.instance.storyModeLevelList = new List<JObject>();

        foreach (var file in fileInfo)
        {
            var newLevel = Instantiate(levelButtonBase.gameObject);
            newLevel.transform.SetParent(levelListReference.transform);
            newLevel.transform.localScale = Vector3.one;
            newLevel.SetActive(true);

            var newLevelButton = newLevel.GetComponent<Button>();
            var stringlevelData = File.ReadAllText(dataPath + file.Name);
            var levelData = JObject.Parse(stringlevelData);
            GameManager.instance.storyModeLevelList.Add(levelData);

            var levelName = levelData["levelName"].ToString() + " (" + GetCollectablesAmount((int)levelData["collectablesObtained"]) + " / " + GetCollectablesAmount((int)levelData["collectablesTotal"]) + ")";
            if ((bool)levelData["wasCompleted"])
            {
                if ((int)levelData["collectablesObtained"] == (int)levelData["collectablesTotal"])
                    newLevel.GetComponent<Image>().color = Color.green;
                else
                    newLevel.GetComponent<Image>().color = Color.yellow;
            }
            newLevel.GetComponentInChildren<Text>().text = levelName;
            newLevel.name = levelData["levelName"].ToString();
            newLevelButton.onClick.AddListener(() => function(levelData));

            levelList.Add(newLevel);
        }
    }

    public void EditLevel(JObject levelData = null)
    {
        menuCamera.Initialize(fadeMaterial, 0f);
        menuCamera.EnableFade(GameManager.fadeTime);
        StartCoroutine(ExecuteEditLevel(levelData));
    }

    IEnumerator ExecuteEditLevel(JObject levelData)
    {
        yield return new WaitForSeconds(GameManager.fadeTime);

        GameManager.instance.LoadLevelData(levelData);
        GameManager.instance.UsingLevelMaker(true);
        SceneManager.LoadScene("Play");
    }

    public void PlayLevel(JObject levelData = null)
    {
        menuCamera.Initialize(fadeMaterial, 0f);
        menuCamera.EnableFade(GameManager.fadeTime);
        StartCoroutine(ExecutePlayLevel(levelData));
    }

    IEnumerator ExecutePlayLevel(JObject levelData)
    {
        yield return new WaitForSeconds(GameManager.fadeTime);

        GameManager.instance.LoadLevelData(levelData);
        GameManager.instance.PlayingLevel(true);
        SceneManager.LoadScene("Play");
    }

    public static void PlayNextLevel(JObject levelData = null)
    {
        GameManager.instance.LoadLevelData(levelData);
        GameManager.instance.PlayingLevel(true);
        SceneManager.LoadScene("Play");
    }

    public void DeleteLevelList(List<GameObject> levelList)
    {
        foreach (var storyModeLevel in levelList)
            Destroy(storyModeLevel);
        levelList.Clear();
    }

    string GetCollectablesAmount(int collectablesAmount)
    {
        var collectableString = "";

        if (collectablesAmount < 10)
            collectableString = "0" + collectablesAmount;
        else
            collectableString = collectablesAmount.ToString();

        return collectableString;
    }
}