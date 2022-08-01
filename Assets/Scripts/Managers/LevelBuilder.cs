using UnityEngine;
using System.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;

public class LevelBuilder : MonoBehaviour
{
    public LayerMask gridCollisionMask;
    public Player player;
    public Tile tile;
    public PassableTile passableTile;
    public MovingPlatform movingPlatform;
    public MovingPlatformVerticalSpikes movingPlatformVerticalSpikes;
    public MovingPlatformHorizontalSpikes movingPlatformHorizontalSpikes;
    public Saw saw;
    public RayShooter rayShooter;
    public GravityChangerPlatform gravityChangerPlatform;
    public Key key;
    public LockedDoor lockedDoor;
    public LevelEnd levelEnd;
    public Spike spike;
    public Collectable collectable;
    public Checkpoint checkpoint;

    private JObject _levelData;
    private int _subLevelBeingBuilt;
    private int _maxSubLevelSteps;
    private int _subLevelSteps;

    void Start()
    {
        if (GameManager.instance.IsPlayingLevel())
        {
            _levelData = GameManager.instance.GetLevelData();
            BuildGameLevel();
        }
        else if (GameManager.instance.IsUsingLevelMaker())
        {
            _levelData = GameManager.instance.GetLevelData();
            if (_levelData != null)
            {
                GameManager.instance.EditingLevel(true);
                BuildEditorLevel();
            }
        }

        GameManager.instance.OnLevelBuildingFinished();
    }

    void BuildGameLevel()
    {
        for (int i = 0; i <= _levelData["subLevelData"].Count() - 1; i++)
        {
            LevelMakerManager.instance.SetActiveSubLevel(i);
            BuildSubLevels(i);
        }

        LevelMakerManager.instance.SetActiveSubLevel(0);
    }

    void BuildEditorLevel()
    {
        if (_subLevelBeingBuilt < LevelMakerManager.instance.subLevelsAmount)
        {
            LevelMakerManager.instance.SetActiveSubLevel(_subLevelBeingBuilt);
            BuildSubLevels(_subLevelBeingBuilt);
        }
        else
            LevelMakerManager.instance.SetActiveSubLevel(0);
    }

    void BuildSubLevels(int index)
    {
        var subLevelData = _levelData["subLevelData"][index];
        BuildPlayerList(subLevelData, index);
        BuildTileList(subLevelData, index);
        BuildPassableTileList(subLevelData, index);
        BuildUnifiedColliderList(subLevelData, index);
        BuildMovingPlatformList(subLevelData, index);
        BuildMovingPlatformVerticalSpikesList(subLevelData, index);
        BuildMovingPlatformHorizontalSpikesList(subLevelData, index);
        BuildSawList(subLevelData, index);
        BuildRayShooterList(subLevelData, index);
        BuildGravityChangerPlatformList(subLevelData, index);
        BuildKeyList(subLevelData, index);
        BuildLockedDoorList(subLevelData, index);
        BuildLevelEndList(subLevelData, index);
        BuildSpikeList(subLevelData, index);
        BuildCollectableList(subLevelData, index);
        BuildCheckpointList(subLevelData, index);
    }

    void BuildPlayerList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["playerData"].Count(); i++)
        {
            var newPrefab = Instantiate(player.gameObject);
            var newPlayer = newPrefab.GetComponent<Player>();
            SetPrefabConfiguration(newPlayer, "Player", index);

            var data = subLevelData["playerData"][i];
            var playerData = new PlayerData();
            LoadTransformData(ref playerData, data);
            LoadGravityEntityData(ref playerData, data);

            newPlayer.LoadData(playerData);
        }
    }

    void BuildTileList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["tileData"].Count(); i++)
        {
            var newPrefab = Instantiate(tile.gameObject);
            var newTile = newPrefab.GetComponent<Tile>();
            SetPrefabConfiguration(newTile, "Tile", index);

            var data = subLevelData["tileData"][i];
            var tileData = new TileData();
            LoadTransformData(ref tileData, data);
            LoadBaseTileData(ref tileData, data);

            newTile.LoadData(tileData);
        }
    }

    void BuildPassableTileList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["passableTileData"].Count(); i++)
        {
            var newPrefab = Instantiate(passableTile.gameObject);
            var newPassableTile = newPrefab.GetComponent<PassableTile>();
            SetPrefabConfiguration(newPassableTile, "Passable Tile", index);

            var data = subLevelData["passableTileData"][i];
            var passableTileData = new PassableTileData();
            LoadTransformData(ref passableTileData, data);
            LoadBaseTileData(ref passableTileData, data);

            newPassableTile.LoadData(passableTileData);
        }
    }

    void BuildUnifiedColliderList(JToken subLevelData, int index)
    {
        if (!GameManager.instance.IsEditingLevel())
        {
            for (int i = 0; i < subLevelData["unifiedColliderData"].Count(); i++)
            {
                var newPrefab = new GameObject();
                newPrefab.AddComponent<BoxCollider2D>();
                var newUnifiedCollider = newPrefab.AddComponent<UnifiedCollider>();
                LevelMakerManager.instance.lmContainers.AddToContainer(newUnifiedCollider.GetType(), newPrefab, index);
                newPrefab.name = "Unified Collider";
                var data = subLevelData["unifiedColliderData"][i];
                var unifiedColliderData = new UnifiedColliderData();

                LoadTransformData(ref unifiedColliderData, data);
                unifiedColliderData.offset = new double[data["offset"].Count()];
                for (int j = 0; j < data["offset"].Count(); j++)
                    unifiedColliderData.offset[j] = (double)data["offset"][j];
                unifiedColliderData.size = new double[data["size"].Count()];
                for (int j = 0; j < data["size"].Count(); j++)
                    unifiedColliderData.size[j] = (double)data["size"][j];
                unifiedColliderData.tag = (string)data["tag"];
                unifiedColliderData.layer = (int)data["layer"];

                newUnifiedCollider.LoadData(unifiedColliderData);
            }
        }
    }

    void BuildMovingPlatformList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["movingPlatformData"].Count(); i++)
        {
            var newPrefab = Instantiate(movingPlatform.gameObject);
            var newMovingPlatform = newPrefab.GetComponent<MovingPlatform>();
            SetPrefabConfiguration(newMovingPlatform, "Moving Platform", index);

            var data = subLevelData["movingPlatformData"][i];
            var movingPlatformData = new MovingPlatformData();
            LoadTransformData(ref movingPlatformData, data);
            LoadMobileBaseData(ref movingPlatformData, data);
            LoadWaypointsData(ref movingPlatformData, data);
            movingPlatformData.platformType = (string)data["platformType"];

            newMovingPlatform.LoadData(movingPlatformData);
        }
    }

    void BuildMovingPlatformVerticalSpikesList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["movingPlatformVerticalSpikesData"].Count(); i++)
        {
            var newPrefab = Instantiate(movingPlatformVerticalSpikes.gameObject);
            var newMovingPlatform = newPrefab.GetComponent<MovingPlatformVerticalSpikes>();
            SetPrefabConfiguration(newMovingPlatform, "Vertical Spikes Moving Platform", index);

            var data = subLevelData["movingPlatformVerticalSpikesData"][i];
            var movingPlatformData = new MovingPlatformVerticalSpikesData();
            LoadTransformData(ref movingPlatformData, data);
            LoadMobileBaseData(ref movingPlatformData, data);
            LoadWaypointsData(ref movingPlatformData, data);
            movingPlatformData.platformType = (string)data["platformType"];

            newMovingPlatform.LoadData(movingPlatformData);
        }
    }

    void BuildMovingPlatformHorizontalSpikesList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["movingPlatformHorizontalSpikesData"].Count(); i++)
        {
            var newPrefab = Instantiate(movingPlatformHorizontalSpikes.gameObject);
            var newMovingPlatform = newPrefab.GetComponent<MovingPlatformHorizontalSpikes>();
            SetPrefabConfiguration(newMovingPlatform, "Horizontal Spikes Moving Platform", index);

            var data = subLevelData["movingPlatformHorizontalSpikesData"][i];
            var movingPlatformData = new MovingPlatformHorizontalSpikesData();
            LoadTransformData(ref movingPlatformData, data);
            LoadMobileBaseData(ref movingPlatformData, data);
            LoadWaypointsData(ref movingPlatformData, data);
            movingPlatformData.platformType = (string)data["platformType"];

            newMovingPlatform.LoadData(movingPlatformData);
        }
    }

    void BuildSawList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["sawData"].Count(); i++)
        {
            var newPrefab = Instantiate(saw.gameObject);
            var newSaw = newPrefab.GetComponent<Saw>();
            SetPrefabConfiguration(newSaw, "Saw", index);

            var data = subLevelData["sawData"][i];
            var sawData = new SawData();
            LoadTransformData(ref sawData, data);
            LoadMobileBaseData(ref sawData, data);
            LoadWaypointsData(ref sawData, data);
            sawData.rotationDirectionName = (string)data["rotationDirectionName"];
            sawData.rotationDirectionIndex = (int)data["rotationDirectionIndex"];
            sawData.rotationSpeed = (double)data["rotationSpeed"];

            newSaw.LoadData(sawData);
        }
    }

    void BuildRayShooterList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["rayShooterData"].Count(); i++)
        {
            var newPrefab = Instantiate(rayShooter.gameObject);
            var newRayShooter = newPrefab.GetComponent<RayShooter>();
            SetPrefabConfiguration(newRayShooter, "Ray Shooter", index);

            var data = subLevelData["rayShooterData"][i];
            var rayShooterData = new RayShooterData();
            LoadTransformData(ref rayShooterData, data);
            LoadMobileBaseData(ref rayShooterData, data);
            LoadWaypointsData(ref rayShooterData, data);
            rayShooterData.rayUp = (bool)data["rayUp"];
            rayShooterData.rayUpIntervalIntermittenceTime = (double)data["rayUpIntervalIntermittenceTime"];
            rayShooterData.rayUpIntervalDurationTime = (double)data["rayUpIntervalDurationTime"];
            rayShooterData.rayRight = (bool)data["rayRight"];
            rayShooterData.rayRightIntervalIntermittenceTime = (double)data["rayRightIntervalIntermittenceTime"];
            rayShooterData.rayRightIntervalDurationTime = (double)data["rayRightIntervalDurationTime"];
            rayShooterData.rayLeft = (bool)data["rayLeft"];
            rayShooterData.rayLeftIntervalIntermittenceTime = (double)data["rayLeftIntervalIntermittenceTime"];
            rayShooterData.rayLeftIntervalDurationTime = (double)data["rayLeftIntervalDurationTime"];
            rayShooterData.rayDown = (bool)data["rayDown"];
            rayShooterData.rayDownIntervalIntermittenceTime = (double)data["rayDownIntervalIntermittenceTime"];
            rayShooterData.rayDownIntervalDurationTime = (double)data["rayDownIntervalDurationTime"];
            rayShooterData.rotationDirectionName = (string)data["rotationDirectionName"];
            rayShooterData.rotationDirectionIndex = (int)data["rotationDirectionIndex"];
            rayShooterData.rotationSpeed = (double)data["rotationSpeed"];

            newRayShooter.LoadData(rayShooterData);
        }
    }

    void BuildGravityChangerPlatformList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["gravityChangerPlatformData"].Count(); i++)
        {
            var newPrefab = Instantiate(gravityChangerPlatform.gameObject);
            var newGravityChangerPlatform = newPrefab.GetComponent<GravityChangerPlatform>();
            SetPrefabConfiguration(newGravityChangerPlatform, "Gravity Changer Platform", index);

            var data = subLevelData["gravityChangerPlatformData"][i];
            var gravityChangerPlatformData = new GravityChangerPlatformData();
            LoadTransformData(ref gravityChangerPlatformData, data);
            gravityChangerPlatformData.nextGravityName = (string)data["nextGravityName"];
            gravityChangerPlatformData.nextGravityIndex = (int)data["nextGravityIndex"];

            newGravityChangerPlatform.LoadData(gravityChangerPlatformData);
        }
    }

    void BuildKeyList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["keyData"].Count(); i++)
        {
            var newPrefab = Instantiate(key.gameObject);
            var newKey = newPrefab.GetComponent<Key>();
            SetPrefabConfiguration(newKey, "Key", index);

            var data = subLevelData["keyData"][i];
            var keyData = new KeyData();
            LoadTransformData(ref keyData, data);
            LoadGravityEntityData(ref keyData, data);

            newKey.LoadData(keyData);
        }
    }

    void BuildLockedDoorList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["lockedDoorData"].Count(); i++)
        {
            var newPrefab = Instantiate(lockedDoor.gameObject);
            var newLockedDoor = newPrefab.GetComponent<LockedDoor>();
            SetPrefabConfiguration(newLockedDoor, "Locked Door", index);

            var data = subLevelData["lockedDoorData"][i];
            var lockedDoorData = new LockedDoorData();
            LoadTransformData(ref lockedDoorData, data);

            newLockedDoor.LoadData(lockedDoorData);
        }
    }

    void BuildLevelEndList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["levelEndData"].Count(); i++)
        {
            var newPrefab = Instantiate(levelEnd.gameObject);
            var newLevelEnd = newPrefab.GetComponent<LevelEnd>();
            SetPrefabConfiguration(newLevelEnd, "Level End", index);

            var data = subLevelData["levelEndData"][i];
            var levelEndData = new LevelEndData();
            LoadTransformData(ref levelEndData, data);

            newLevelEnd.LoadData(levelEndData);
        }
    }

    void BuildSpikeList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["spikeData"].Count(); i++)
        {
            var newPrefab = Instantiate(spike.gameObject);
            var newSpike = newPrefab.GetComponent<Spike>();
            SetPrefabConfiguration(newSpike, "Spike", index);

            var data = subLevelData["spikeData"][i];
            var spikeData = new SpikeData();
            LoadTransformData(ref spikeData, data);

            newSpike.LoadData(spikeData);
        }
    }

    void BuildCollectableList(JToken subLevelData, int index)
    {
        for (int i = 0; i < subLevelData["collectableData"].Count(); i++)
        {
            var newPrefab = Instantiate(collectable.gameObject);
            var newCollectable = newPrefab.GetComponent<Collectable>();
            SetPrefabConfiguration(newCollectable, "Collectable", index);

            var data = subLevelData["collectableData"][i];
            var collectableData = new CollectableData();
            LoadTransformData(ref collectableData, data);

            newCollectable.LoadData(collectableData);
        }
    }

    void BuildCheckpointList(JToken subLevelData, int index)
    {
        if (subLevelData["checkpointData"] == null)
            return;

        for (int i = 0; i < subLevelData["checkpointData"].Count(); i++)
        {
            var newPrefab = Instantiate(checkpoint.gameObject);
            var newCheckpoint = newPrefab.GetComponent<Checkpoint>();
            SetPrefabConfiguration(newCheckpoint, "Checkpoint", index);

            var data = subLevelData["checkpointData"][i];
            var checkpointData = new CheckpointData();
            LoadTransformData(ref checkpointData, data);

            newCheckpoint.LoadData(checkpointData);
        }
    }

    void SetPrefabConfiguration<T>(T basePrefab, string basePrefabName, int index) where T : BasePrefab
    {
        if (GameManager.instance.IsEditingLevel())
        {
            basePrefab.needsToBeCreated = false;
            CheckLimitedBasePrefab(basePrefab, index);
            StartCoroutine(FillPrefabCollidingGridCells(basePrefab));
        }
        LevelMakerManager.instance.lmBasePrefabLayerSorter.SetSortingLayer(basePrefab);
        LevelMakerManager.instance.lmContainers.AddToContainer(basePrefab.GetType(), basePrefab.gameObject, index);
        basePrefab.name = basePrefabName;
    }

    void LoadTransformData<T>(ref T transformData, JToken data) where T : TransformData
    {
        transformData.position = new double[data["position"].Count()];
        for (int i = 0; i < data["position"].Count(); i++)
            transformData.position[i] = (double)data["position"][i];

        transformData.rotation = new double[data["rotation"].Count()];
        for (int i = 0; i < data["rotation"].Count(); i++)
            transformData.rotation[i] = (double)data["rotation"][i];
    }

    void LoadBaseTileData<T>(ref T baseTileData, JToken data) where T : BaseTileData
    {
        baseTileData.destroyable = (bool)data["destroyable"];
        baseTileData.alone = (bool)data["alone"];
        baseTileData.emissionUpEnabled = (bool)data["emissionUpEnabled"];
        baseTileData.emissionRightEnabled = (bool)data["emissionRightEnabled"];
        baseTileData.emissionLeftEnabled = (bool)data["emissionLeftEnabled"];
        baseTileData.emissionDownEnabled = (bool)data["emissionDownEnabled"];
    }

    void LoadGravityEntityData<T>(ref T gravityEntityData, JToken data) where T : GravityEntityData
    {
        gravityEntityData.currentGravity = new double[data["currentGravity"].Count()];
        for (int i = 0; i < data["currentGravity"].Count(); i++)
            gravityEntityData.currentGravity[i] = (double)data["currentGravity"][i];
    }

    void LoadMobileBaseData<T>(ref T mobileBaseData, JToken data) where T : MobileBaseData
    {
        mobileBaseData.speed = (double)data["speed"];
        mobileBaseData.waitTime = (double)data["waitTime"];
        mobileBaseData.startTime = (double)data["startTime"];
        mobileBaseData.cyclic = (bool)data["cyclic"];
    }

    void LoadWaypointsData<T1>(ref T1 mobileBaseData, JToken data) where T1 : MobileBaseData
    {
        mobileBaseData.waypointData = new WaypointData[data["waypointData"].Count()];

        for (int i = 0; i < data["waypointData"].Count(); i++)
        {
            var waypointData = data["waypointData"][i];

            mobileBaseData.waypointData[i] = new WaypointData();
            mobileBaseData.waypointData[i].index = (int)waypointData["index"];

            mobileBaseData.waypointData[i].position = new double[waypointData["position"].Count()];
            for (int j = 0; j < waypointData["position"].Count(); j++)
                mobileBaseData.waypointData[i].position[j] = (double)waypointData["position"][j];

            mobileBaseData.waypointData[i].rotation = new double[waypointData["rotation"].Count()];
            for (int j = 0; j < waypointData["rotation"].Count(); j++)
                mobileBaseData.waypointData[i].rotation[j] = (double)waypointData["rotation"][j];
        }
    }

    IEnumerator FillPrefabCollidingGridCells(BasePrefab basePrefab)
    {
        yield return new WaitForSeconds(0.05f);

        var gridCells = LevelMakerManager.instance.levelMakerSettings.GetGridCellsHits(basePrefab);
        if (gridCells.Count > 0)
        {
            for (int i = 0; i < gridCells.Count; i++)
            {
                if (basePrefab.GetComponent<BaseTile>())
                    gridCells[i].SetGridTileFillment(basePrefab.GetComponent<BaseTile>(), false);
                else
                {
                    gridCells[i].SetGridPrefabFillment(basePrefab, false);
                    if (basePrefab.spawnAmountLimit > 0)
                        basePrefab.SetGridCells(gridCells[i]);
                }
            }
        }

        _subLevelSteps++;
        CheckSubLevelSteps();
    }

    void CheckLimitedBasePrefab(BasePrefab basePrefab, int index)
    {
        if (basePrefab.spawnAmountLimit > 0)
        {
            basePrefab.spawnedSubLevel = index;
            LevelMakerManager.instance.levelMakerSettings.lmBasePrefabLimiter.CheckLimitedBasePrefab(basePrefab);
        }
    }

    void CheckSubLevelSteps()
    {
        if (_subLevelSteps >= _maxSubLevelSteps)
        {
            _subLevelBeingBuilt++;
            _maxSubLevelSteps = 0;
            _subLevelSteps = 0;
            BuildEditorLevel();
        }
    }
}