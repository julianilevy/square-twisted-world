using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

public class ProgressManager : MonoBehaviour
{
    [HideInInspector]
    public static ProgressManager instance;
    [HideInInspector]
    public int collectablesObtained;

    void Start()
    {
        if (instance == null)
            instance = this;
        GameManager.instance.GetLevelData();
    }

    public void AddCollectable()
    {
        collectablesObtained++;
        CanvasUIManager.instance.UpdateUI();
    }

    public void SaveProgress()
    {
        var levelData = GameManager.instance.GetLevelData();
        levelData["wasCompleted"] = true;
        if ((int)levelData["collectablesObtained"]  < collectablesObtained)
            levelData["collectablesObtained"] = collectablesObtained;

        var dataPath = "";
        if (GameManager.instance.lastLevelWasStory)
            dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Square Twisted World/Levels/Story Mode/";
        else
            dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Square Twisted World/Levels/Custom/";
        var directoryInfo = new DirectoryInfo(dataPath);
        var fileInfo = directoryInfo.GetFiles("*.json");

        File.WriteAllText(dataPath + levelData["levelName"] + ".json", JsonConvert.SerializeObject(levelData, Formatting.Indented));
    }
}