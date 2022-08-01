using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LMDragAndDropOnGrid : MonoBehaviour
{
    [HideInInspector]
    public BasePrefab currentDraggedPrefab;
    [HideInInspector]
    public Material abledGridCellMaterial;
    [HideInInspector]
    public Material unabledGridCellMaterial;
    [HideInInspector]
    public bool movingPlacedPrefab;

    private LMSettings _lmSettings;
    private List<GridCell> _currentGridCells;
    private Vector3 _currentGridPosition;
    private Vector3 _currentPrefabPosition;
    private bool _changedCurrentGridPosition;

    void Awake()
    {
        _lmSettings = GetComponent<LMSettings>();
        _lmSettings.AddToOnGUIFunctions(DetectCurrentGridCell);
        _lmSettings.AddToOnGUIFunctions(DetectCollidingGridCells);
        _lmSettings.AddToOnGUIFunctions(CreateContinuous);
        _lmSettings.AddToOnGUIFunctions(MoveDraggedPrefab);
        _lmSettings.AddToUpdateFunctions(CreateDiscrete);
        _lmSettings.AddToUpdateFunctions(RotatePrefab);
        _lmSettings.AddToUpdateFunctions(DeleteDraggedPrefab);
        _currentGridCells = new List<GridCell>();
    }

    void DetectCurrentGridCell()
    {
        if (currentDraggedPrefab != null)
        {
            var hit = _lmSettings.GetGridCellClicked();

            if (hit)
            {
                _changedCurrentGridPosition = false;

                if (!currentDraggedPrefab.snapToTile)
                {
                    if (_currentGridPosition != hit.transform.position)
                    {
                        _changedCurrentGridPosition = true;
                        _currentGridPosition = hit.transform.position;
                    }
                }
                else
                {
                    var snapValue = currentDraggedPrefab.GetSnapValueByRotation(currentDraggedPrefab.transform.rotation);

                    if (_currentGridPosition != hit.transform.position + snapValue)
                    {
                        _changedCurrentGridPosition = true;
                        _currentGridPosition = hit.transform.position + snapValue;
                    }
                }
            }
        }
    }

    void DetectCollidingGridCells()
    {
        if (currentDraggedPrefab != null)
        {
            if (_currentPrefabPosition == currentDraggedPrefab.transform.position)
                return;

            _currentPrefabPosition = currentDraggedPrefab.transform.position;

            var hits = new List<Collider2D>();
            var currentPrefabChildren = currentDraggedPrefab.GetComponentsInChildren<BasePrefab>();

            foreach (var prefabChild in currentPrefabChildren)
            {
                if (prefabChild.beingDragged)
                {
                    foreach (var collider in prefabChild.GetColliderHits(_lmSettings.gridCollisionMask))
                        hits.Add(collider);
                }
            }

            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                    _currentGridCells.Add(hits[i].gameObject.GetComponent<GridCell>());

                if (!_changedCurrentGridPosition)
                {
                    foreach (var currentGridCell in _currentGridCells)
                    {
                        currentGridCell.inside.gameObject.SetActive(true);

                        if (currentDraggedPrefab.GetComponent<BaseTile>())
                        {
                            if (!currentGridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel])
                                currentGridCell.inside.material = abledGridCellMaterial;
                            else
                                currentGridCell.inside.material = unabledGridCellMaterial;
                        }
                        else
                        {
                            if (!currentGridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel])
                                currentGridCell.inside.material = abledGridCellMaterial;
                            else
                                currentGridCell.inside.material = unabledGridCellMaterial;
                        }
                    }
                }
                else
                {
                    foreach (var currentGridCell in _currentGridCells)
                        currentGridCell.inside.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var currentGridCell in _currentGridCells)
            {
                if (currentGridCell != null)
                    currentGridCell.inside.gameObject.SetActive(false);
            }
            _currentPrefabPosition = Vector3.zero;
            _currentGridCells.Clear();
        }
    }

    void CreateDiscrete()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_lmSettings.IsCollidingWithUI())
            {
                if (currentDraggedPrefab != null)
                {
                    if (!currentDraggedPrefab.canContinuousPlacing)
                        CreateOrPlacePrefab();
                }
            }
        }
    }

    void CreateContinuous()
    {
        if (Input.GetMouseButton(0))
        {
            if (!_lmSettings.IsCollidingWithUI())
            {
                if (currentDraggedPrefab != null)
                {
                    if (currentDraggedPrefab.canContinuousPlacing)
                    {
                        if (currentDraggedPrefab.GetComponent<BaseTile>())
                            CreateOrPlaceTile();
                        else
                            CreateOrPlacePrefab();
                    }
                }
            }
        }
        if (_changedCurrentGridPosition)
        {
            foreach (var currentGridCell in _currentGridCells)
                currentGridCell.inside.gameObject.SetActive(false);
            _currentGridCells.Clear();
        }
    }

    void RotatePrefab()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (currentDraggedPrefab != null)
            {
                if (currentDraggedPrefab.canBeRotated)
                {
                    if (currentDraggedPrefab.gameObject.layer == K.LAYER_MOVINGPLATFORM)
                    {
                        if (currentDraggedPrefab.GetComponent<MovingPlatform>())
                            currentDraggedPrefab.GetComponent<MovingPlatform>().Rotate();
                        else if (currentDraggedPrefab.GetComponent<MovingPlatformVerticalSpikes>())
                            currentDraggedPrefab.GetComponent<MovingPlatformVerticalSpikes>().Rotate();
                        else if (currentDraggedPrefab.GetComponent<MovingPlatformHorizontalSpikes>())
                            currentDraggedPrefab.GetComponent<MovingPlatformHorizontalSpikes>().Rotate();
                    }
                    else if (currentDraggedPrefab.gameObject.layer == K.LAYER_CHECKPOINT)
                    {
                        var rotationAngle = new Vector3(0f, 0f, 90f);
                        currentDraggedPrefab.transform.Rotate(rotationAngle);
                        if (currentDraggedPrefab.GetComponent<Checkpoint>())
                            currentDraggedPrefab.GetComponent<Checkpoint>().Rotate();
                    }
                    else
                    {
                        var rotationAngle = new Vector3(0f, 0f, 90f);
                        currentDraggedPrefab.transform.Rotate(rotationAngle);
                        if (currentDraggedPrefab.GetComponent<GravityEntity>())
                            currentDraggedPrefab.GetComponent<GravityEntity>().EqualRotationToGravity();
                    }

                    foreach (var currentGridCell in _currentGridCells)
                        currentGridCell.inside.gameObject.SetActive(false);
                    _currentGridCells.Clear();
                    ForceCollidingGridCellsDetection();
                }
            }
        }
    }

    void MoveDraggedPrefab()
    {
        if (currentDraggedPrefab != null)
            currentDraggedPrefab.transform.position = _currentGridPosition;
    }

    void DeleteDraggedPrefab()
    {
        if (Input.GetMouseButtonDown(1))
            _lmSettings.Delete(ref currentDraggedPrefab);
    }

    void SetPrefabGrabbedBool(GameObject prefab, bool value)
    {
        var prefabChildren = prefab.GetComponentsInChildren<BasePrefab>();
        foreach (var prefabChild in prefabChildren)
            prefabChild.beingDragged = value;
    }

    void CreateOrPlaceTile()
    {
        var canSpawnTile = true;

        foreach (var currentGridCell in _currentGridCells)
        {
            if (currentGridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel])
            {
                canSpawnTile = false;
                break;
            }
        }

        if (!canSpawnTile)
        {
            foreach (var currentGridCell in _currentGridCells)
                currentGridCell.CheckGridTileFillment();
            return;
        }

        if (!movingPlacedPrefab)
        {
            var newTileGO = (Instantiate(currentDraggedPrefab.gameObject, _currentGridPosition, currentDraggedPrefab.transform.rotation));
            newTileGO.name = currentDraggedPrefab.gameObject.name;
            SetPrefabGrabbedBool(newTileGO, false);
            var newTile = newTileGO.GetComponent<BaseTile>();
            newTile.RemoveEmissiveEdges();
            LevelMakerManager.instance.lmBasePrefabLayerSorter.SetSortingLayer(newTile);
            LevelMakerManager.instance.lmContainers.AddToContainer(newTile.GetType(), newTile.gameObject);

            ForceCollidingGridCellsDetection();
            foreach (var currentGridCell in _currentGridCells)
                currentGridCell.SetGridTileFillment(newTile);
            ForceCollidingGridCellsDetection();
        }
        else
        {
            if (currentDraggedPrefab.canBePlacedAfterPickUp)
            {
                ForceCollidingGridCellsDetection();
                var currentDraggedTile = currentDraggedPrefab.GetComponent<BaseTile>();
                foreach (var currentGridCell in _currentGridCells)
                    currentGridCell.SetGridTileFillment(currentDraggedTile);
                ForceCollidingGridCellsDetection();

                LevelMakerManager.instance.lmContainers.AddToContainer(currentDraggedTile.GetType(), currentDraggedTile.gameObject);
                SetPrefabGrabbedBool(currentDraggedTile.gameObject, false);
                StopMovingPlacedPrefab();
                currentDraggedTile.RemoveEmissiveEdges();
                StartCoroutine(DeselectMovingTileAtEndOfFrame());
            }
        }
    }

    void CreateOrPlacePrefab()
    {
        var canSpawnPrefab = true;

        if (currentDraggedPrefab.canTileOverlap)
        {
            foreach (var currentGridCell in _currentGridCells)
            {
                if (currentGridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel])
                {
                    canSpawnPrefab = false;
                    break;
                }
            }
        }
        else
        {
            foreach (var currentGridCell in _currentGridCells)
            {
                if (currentGridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel] || currentGridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel])
                {
                    canSpawnPrefab = false;
                    break;
                }
            }
        }

        if (!canSpawnPrefab)
        {
            foreach (var currentGridCell in _currentGridCells)
                currentGridCell.CheckGridPrefabFillment();
            return;
        }

        if (!movingPlacedPrefab)
        {
            var newPrefabGO = (Instantiate(currentDraggedPrefab.gameObject, _currentGridPosition, currentDraggedPrefab.transform.rotation));
            newPrefabGO.name = currentDraggedPrefab.gameObject.name;
            SetPrefabGrabbedBool(newPrefabGO, false);
            var newPrefab = newPrefabGO.GetComponent<BasePrefab>();
            CheckLimitedBasePrefab(newPrefab);
            LevelMakerManager.instance.lmBasePrefabLayerSorter.SetSortingLayer(newPrefab);
            LevelMakerManager.instance.lmContainers.AddToContainer(newPrefab.GetType(), newPrefab.gameObject);

            if (newPrefab.gameObject.layer == K.LAYER_MOVINGPLATFORM)
            {
                if (newPrefab.GetComponent<MovingPlatform>())
                    newPrefab.GetComponent<MovingPlatform>().SetType(currentDraggedPrefab.GetComponent<MovingPlatform>().platformType);
                else if (newPrefab.GetComponent<MovingPlatformVerticalSpikes>())
                    newPrefab.GetComponent<MovingPlatformVerticalSpikes>().SetType(currentDraggedPrefab.GetComponent<MovingPlatformVerticalSpikes>().platformType);
                else if (newPrefab.GetComponent<MovingPlatformHorizontalSpikes>())
                    newPrefab.GetComponent<MovingPlatformHorizontalSpikes>().SetType(currentDraggedPrefab.GetComponent<MovingPlatformHorizontalSpikes>().platformType);
            }

            ForceCollidingGridCellsDetection();
            foreach (var currentGridCell in _currentGridCells)
                currentGridCell.SetGridPrefabFillment(newPrefab);
            ForceCollidingGridCellsDetection();
        }
        else
        {
            if (currentDraggedPrefab.canBePlacedAfterPickUp)
            {
                ForceCollidingGridCellsDetection();
                foreach (var currentGridCell in _currentGridCells)
                    currentGridCell.SetGridPrefabFillment(currentDraggedPrefab);
                ForceCollidingGridCellsDetection();

                LevelMakerManager.instance.lmContainers.AddToContainer(currentDraggedPrefab.GetType(), currentDraggedPrefab.gameObject);
                CheckLimitedBasePrefab(currentDraggedPrefab, true);
                SetPrefabGrabbedBool(currentDraggedPrefab.gameObject, false);
                StopMovingPlacedPrefab();
            }
        }
    }

    void StopMovingPlacedPrefab()
    {
        currentDraggedPrefab.originalPosition = currentDraggedPrefab.transform.position;
        currentDraggedPrefab.originalRotation = currentDraggedPrefab.transform.rotation;
        currentDraggedPrefab = null;
        movingPlacedPrefab = false;
    }

    void ForceCollidingGridCellsDetection()
    {
        if (currentDraggedPrefab != null)
        {
            var hits = new List<Collider2D>();
            var currentPrefabChildren = currentDraggedPrefab.GetComponentsInChildren<BasePrefab>();

            foreach (var prefabChild in currentPrefabChildren)
            {
                if (prefabChild.beingDragged)
                {
                    foreach (var collider in prefabChild.GetColliderHits(_lmSettings.gridCollisionMask))
                        hits.Add(collider);
                }
            }

            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                    _currentGridCells.Add(hits[i].gameObject.GetComponent<GridCell>());

                if (!_changedCurrentGridPosition)
                {
                    foreach (var currentGridCell in _currentGridCells)
                    {
                        currentGridCell.inside.gameObject.SetActive(true);

                        if (currentDraggedPrefab.GetComponent<BaseTile>())
                        {
                            if (!currentGridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel])
                                currentGridCell.inside.material = abledGridCellMaterial;
                            else
                                currentGridCell.inside.material = unabledGridCellMaterial;
                        }
                        else
                        {
                            if (!currentGridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel])
                                currentGridCell.inside.material = abledGridCellMaterial;
                            else
                                currentGridCell.inside.material = unabledGridCellMaterial;
                        }
                    }
                }
                else
                {
                    foreach (var currentGridCell in _currentGridCells)
                        currentGridCell.inside.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var currentGridCell in _currentGridCells)
            {
                if (currentGridCell != null)
                    currentGridCell.inside.gameObject.SetActive(false);
            }
            _currentPrefabPosition = Vector3.zero;
            _currentGridCells.Clear();
        }
    }

    public void SetPrefabOnMouse(BasePrefab grabbedPrefab)
    {
        _lmSettings.DeselectAndDeleteAll();
        currentDraggedPrefab = grabbedPrefab;
        currentDraggedPrefab.StartMovingPrefab();
        SetPrefabGrabbedBool(currentDraggedPrefab.gameObject, true);
        StartCoroutine(ForceCollidingGridCellsDetectionAtEndOfFrame());
    }

    public void SetPrefabOnMouse(GameObject prefab) // Llamado desde el botón...
    {
        _lmSettings.DeselectAndDeleteAll();
        var newPrefab = (Instantiate(prefab.gameObject, Input.mousePosition, prefab.transform.rotation));
        newPrefab.name = prefab.name;
        currentDraggedPrefab = newPrefab.GetComponent<BasePrefab>();
        if (!currentDraggedPrefab.GetComponent<BaseTile>())
            LevelMakerManager.instance.lmBasePrefabLayerSorter.SetMaxSortingOrder(currentDraggedPrefab);
        SetPrefabGrabbedBool(currentDraggedPrefab.gameObject, true);
        if (currentDraggedPrefab.gameObject.layer == K.LAYER_PLAYER)
        {
            var playerCamera = currentDraggedPrefab.GetComponent<Player>().playerCamera;
            playerCamera.gameObject.SetActive(false);
        }
    }

    void CheckLimitedBasePrefab(BasePrefab basePrefab, bool movingPrefab = false)
    {
        if (basePrefab.spawnAmountLimit > 0)
        {
            basePrefab.spawnedSubLevel = LevelMakerManager.instance.currentSubLevel;
            basePrefab.SetGridCells();
            _lmSettings.lmBasePrefabLimiter.CheckLimitedBasePrefab(basePrefab, movingPrefab);
        }
    }

    IEnumerator ForceCollidingGridCellsDetectionAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        ForceCollidingGridCellsDetection();
    }

    IEnumerator DeselectMovingTileAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        _lmSettings.DeselectPlacedPrefab();
    }
}