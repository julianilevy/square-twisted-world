using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LMSelectPlacedPrefab : MonoBehaviour
{
    private LMSettings _lmSettings;

    void Awake()
    {
        _lmSettings = GetComponent<LMSettings>();
        _lmSettings.AddToUpdateFunctions(ClickOnPlacedPrefab);
        _lmSettings.AddToUpdateFunctions(DeselectPlacedPrefab);
    }

    void ClickOnPlacedPrefab()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_lmSettings.IsCollidingWithUI())
            {
                if (_lmSettings.lmDragAndDropOnGrid.currentDraggedPrefab == null && _lmSettings.lmEraser.eraser == null)
                {
                    if (_lmSettings.lmModifiableList.currentSelectedPrefab == null)
                        SelectPlacedPrefab();
                    else
                    {
                        if (_lmSettings.SelectedPrefabIsMobilePrefab())
                        {
                            if (_lmSettings.lmPrefabWaypointCreator.currentWaypoint == null)
                            {
                                if (!Input.GetKey(KeyCode.LeftShift))
                                    PickUpPlacedPrefab();
                            }
                        }
                        else
                            PickUpPlacedPrefab();
                    }
                }
            }
        }
    }

    void DeselectPlacedPrefab()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!_lmSettings.lmDragAndDropOnGrid.movingPlacedPrefab)
            {
                if (_lmSettings.lmPrefabWaypointCreator.ableToDeselectCurrentPrefab)
                    _lmSettings.DeselectPlacedPrefab();
            }
            else
                _lmSettings.DeselectAndDeleteAll();
        }
    }

    void SelectPlacedPrefab()
    {
        var hit = _lmSettings.GetGridCellClicked();
        if (hit)
        {
            var gridCell = hit.collider.GetComponent<GridCell>();

            if (gridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel] || gridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel])
            {
                if (_lmSettings.lmPrefabWaypointCreator.IsWaypointEditorModeOn())
                    return;

                var gridCellHits = gridCell.GetAllGridCellHits(_lmSettings.prefabsCollisionMask);
                if (gridCellHits != null && gridCellHits.Length > 0)
                {
                    var basePrefabAhead = GetBasePrefabAhead(gridCellHits);
                    _lmSettings.lmModifiableList.currentSelectedPrefab = basePrefabAhead.GetComponentInParent<LMBasePrefabModifable>();
                    _lmSettings.lmModifiableList.CreateModifableList();
                }
                else
                {
                    if (gridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel])
                    {
                        gridCell.currentTiles[LevelMakerManager.instance.currentSubLevel] = null;
                        gridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel] = false;
                    }
                    if (gridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel])
                    {
                        gridCell.currentPrefabs[LevelMakerManager.instance.currentSubLevel] = null;
                        gridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel] = false;
                    }
                }
            }
            else
                _lmSettings.DeselectPlacedPrefab();
        }
    }

    void PickUpPlacedPrefab()
    {
        var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab;
        LMBasePrefabModifable nextSelectedPrefab = null;

        if (_lmSettings.lmPrefabWaypointCreator.ableToDeselectCurrentPrefab)
            _lmSettings.DeselectPlacedPrefab();

        var hit = _lmSettings.GetGridCellClicked();
        if (hit)
        {
            var gridCell = hit.collider.GetComponent<GridCell>();

            if (gridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel] || gridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel])
            {
                if (_lmSettings.lmPrefabWaypointCreator.IsWaypointEditorModeOn())
                    return;

                var gridCellHits = gridCell.GetAllGridCellHits(_lmSettings.prefabsCollisionMask);
                if (gridCellHits != null && gridCellHits.Length > 0)
                {
                    var basePrefabAhead = GetBasePrefabAhead(gridCellHits);
                    nextSelectedPrefab = basePrefabAhead.GetComponentInParent<LMBasePrefabModifable>();
                }
            }
            else
            {
                if (_lmSettings.lmPrefabWaypointCreator.ableToDeselectCurrentPrefab)
                    _lmSettings.DeselectPlacedPrefab();
            }
        }

        if (nextSelectedPrefab != null)
        {
            if (currentSelectedPrefab == nextSelectedPrefab)
            {
                var currentPrefabModifiableChildren = currentSelectedPrefab.GetComponentsInChildren<BasePrefab>();

                foreach (var prefabModifiableChild in currentPrefabModifiableChildren)
                {
                    var colliderHits = prefabModifiableChild.GetColliderHits(_lmSettings.gridCollisionMask);

                    foreach (var colliderHit in colliderHits)
                    {
                        var gridCell = colliderHit.GetComponent<GridCell>();

                        if (!currentSelectedPrefab.GetComponent<BasePrefab>().canTileOverlap)
                        {
                            gridCell.currentPrefabs[LevelMakerManager.instance.currentSubLevel] = null;
                            gridCell.currentTiles[LevelMakerManager.instance.currentSubLevel] = null;
                            gridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel] = false;
                            gridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel] = false;
                        }
                        else
                        {
                            gridCell.currentPrefabs[LevelMakerManager.instance.currentSubLevel] = null;
                            gridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel] = false;
                        }
                    }
                }

                if (currentSelectedPrefab.GetComponent<BaseTile>())
                    StartCoroutine(currentSelectedPrefab.GetComponent<BaseTile>().PickUpProcess());

                _lmSettings.lmDragAndDropOnGrid.SetPrefabOnMouse(currentSelectedPrefab.GetComponent<BasePrefab>());
                _lmSettings.lmDragAndDropOnGrid.movingPlacedPrefab = true;
            }
            else
            {
                _lmSettings.lmModifiableList.currentSelectedPrefab = nextSelectedPrefab;
                _lmSettings.lmModifiableList.CreateModifableList();
            }
        }
    }

    BasePrefab GetBasePrefabAhead(Collider2D[] gridCellHits)
    {
        BasePrefab basePrefabAhead = null;

        List<BasePrefab> basePrefabList = new List<BasePrefab>();
        for (int i = 0; i < gridCellHits.Length; i++)
            basePrefabList.Add(gridCellHits[i].GetComponentInParent<BasePrefab>());

        if (basePrefabList.Count == 1)
            basePrefabAhead = basePrefabList[0];
        else
        {
            var frontSortingLayer = basePrefabList.Max(t => t.sortingLayerOrder);
            foreach (var basePrefab in basePrefabList)
            {
                if (basePrefab.sortingLayerOrder == frontSortingLayer)
                {
                    basePrefabAhead = basePrefab;
                    break;
                }
            }
        }

        return basePrefabAhead;
    }
}