using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;

public class LMSaver : MonoBehaviour
{
    private LMSettings _lmSettings;
    private LevelData _levelData;
    private int _remainingPreSaveSteps = 2;
    private int _initialSubLevel;

    void Awake()
    {
        _lmSettings = GetComponent<LMSettings>();
    }

    public void SaveLevel() // Llamado desde el botón...
    {
        ContinuePreSavingProcess(true);
    }

    public void ContinuePreSavingProcess(bool firstEntry = false)
    {
        if (firstEntry)
            _initialSubLevel = LevelMakerManager.instance.currentSubLevel;
        else
            _remainingPreSaveSteps--;

        switch (_remainingPreSaveSteps)
        {
            case 2:
                _lmSettings.lmTileEdgeChecker.CheckAllTiles();
                break;
            case 1:
                _lmSettings.lmColliderUnifier.UnifyColliders();
                break;
            case 0:
                PreSavingProcessEnded();
                break;
        }
    }

    void PreSavingProcessEnded()
    {
        if (GameManager.instance.adminMode)
            SaveLevel(true);
        else
            SaveLevel(false);
    }

    void SaveLevel(bool isStoryModeLevel)
    {
        var dataPath = "";
        if (isStoryModeLevel)
            dataPath = Application.streamingAssetsPath + "/Levels/Story Mode/";
        else
            dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Square Twisted World/Levels/Custom/";
        var directoryInfo = new DirectoryInfo(dataPath);
        var fileInfo = directoryInfo.GetFiles("*.json");
        var levelID = GetLevelID(fileInfo);
        var levelName = GetLevelName(fileInfo);

        _levelData = new LevelData();
        _levelData.levelID = levelID;
        _levelData.levelName = levelName;
        SaveLevelData();
        File.WriteAllText(dataPath + levelName + ".json", JsonConvert.SerializeObject(_levelData, Formatting.Indented));

        _remainingPreSaveSteps = 2;
        _lmSettings.lmColliderUnifier.DestroyCollidersContainer();
        LevelMakerManager.instance.SetActiveSubLevel(_initialSubLevel);
    }

    void SaveLevelData()
    {
        if (_levelData != null)
        {
            _levelData.subLevelData = new SubLevelData[LevelMakerManager.instance.subLevelsAmount];

            for (int i = 0; i < LevelMakerManager.instance.subLevelsAmount; i++)
            {
                LevelMakerManager.instance.SetActiveSubLevel(i);

                var subLevelData = new SubLevelData();
                SaveData<Player>(ref subLevelData.playerData, i);
                SaveData<Tile>(ref subLevelData.tileData, i);
                SaveData<PassableTile>(ref subLevelData.passableTileData, i);
                SaveData<UnifiedCollider>(ref subLevelData.unifiedColliderData, i);
                SaveData<MovingPlatform>(ref subLevelData.movingPlatformData, i);
                SaveData<MovingPlatformVerticalSpikes>(ref subLevelData.movingPlatformVerticalSpikesData, i);
                SaveData<MovingPlatformHorizontalSpikes>(ref subLevelData.movingPlatformHorizontalSpikesData, i);
                SaveData<Tile>(ref subLevelData.tileData, i);
                SaveData<Saw>(ref subLevelData.sawData, i);
                SaveData<RayShooter>(ref subLevelData.rayShooterData, i);
                SaveData<GravityChangerPlatform>(ref subLevelData.gravityChangerPlatformData, i);
                SaveData<Key>(ref subLevelData.keyData, i);
                SaveData<LockedDoor>(ref subLevelData.lockedDoorData, i);
                SaveData<LevelEnd>(ref subLevelData.levelEndData, i);
                SaveData<Spike>(ref subLevelData.spikeData, i);
                SaveData<Collectable>(ref subLevelData.collectableData, i);
                SaveData<Checkpoint>(ref subLevelData.checkpointData, i);
                _levelData.collectablesTotal += LevelMakerManager.instance.lmContainers.containers[i][typeof(Collectable)].transform.childCount;
                _levelData.subLevelData[i] = subLevelData;
            }
        }
    }

    void SaveData<T>(ref TransformData[] dataArray, int index) where T : ISavable
    {
        var currentContainer = LevelMakerManager.instance.lmContainers.containers[index][typeof(T)];
        var currentTypeList = new List<T>();
        dataArray = new TransformData[currentContainer.transform.childCount];

        foreach (Transform containerChild in currentContainer.transform)
            currentTypeList.Add(containerChild.GetComponent<T>());

        for (int i = 0; i < currentTypeList.Count; i++)
        {
            currentTypeList[i].SaveData();
            dataArray[i] = currentTypeList[i].GetSavedData();
        }
    }

    int GetLevelID(FileInfo[] fileInfo)
    {
        var levelID = fileInfo.Length + 1;
        return levelID;
    }

    string GetLevelName(FileInfo[] fileInfo)
    {
        var levelName = "";

        if (fileInfo.Length == 0)
            levelName = "Level 01";
        else
        {
            var levelNumber = fileInfo.Length + 1;

            if (levelNumber < 10)
                levelName = "Level 0" + levelNumber;
            else
                levelName = "Level " + levelNumber;
        }

        return levelName;
    }
}