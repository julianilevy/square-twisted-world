using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LMDragAndDropOnGrid))]
[RequireComponent(typeof(LMSelectPlacedPrefab))]
[RequireComponent(typeof(LMModifiableList))]
[RequireComponent(typeof(LMPrefabWaypointCreator))]
[RequireComponent(typeof(LMEraser))]
[RequireComponent(typeof(LMColliderUnifier))]
[RequireComponent(typeof(LMTileEdgeChecker))]
[RequireComponent(typeof(LMSaver))]
[RequireComponent(typeof(LMBasePrefabLimiter))]
public class LMSettings : MonoBehaviour
{
    public LayerMask gridCollisionMask;
    public LayerMask waypointsCollisionMask;
    public LayerMask prefabsCollisionMask;

    [HideInInspector]
    public Camera lmCamera;
    [HideInInspector]
    public LMDragAndDropOnGrid lmDragAndDropOnGrid;
    [HideInInspector]
    public LMSelectPlacedPrefab lmSelectPlacedPrefab;
    [HideInInspector]
    public LMModifiableList lmModifiableList;
    [HideInInspector]
    public LMPrefabWaypointCreator lmPrefabWaypointCreator;
    [HideInInspector]
    public LMEraser lmEraser;
    [HideInInspector]
    public LMColliderUnifier lmColliderUnifier;
    [HideInInspector]
    public LMTileEdgeChecker lmTileEdgeChecker;
    [HideInInspector]
    public LMSaver lmSaver;
    [HideInInspector]
    public LMBasePrefabLimiter lmBasePrefabLimiter;

    private List<Action> _updateFunctions;
    private List<Action> _onGUIFunctions;

    void Awake()
    {
        lmDragAndDropOnGrid = GetComponent<LMDragAndDropOnGrid>();
        lmSelectPlacedPrefab = GetComponent<LMSelectPlacedPrefab>();
        lmModifiableList = GetComponent<LMModifiableList>();
        lmPrefabWaypointCreator = GetComponent<LMPrefabWaypointCreator>();
        lmEraser = GetComponent<LMEraser>();
        lmColliderUnifier = GetComponent<LMColliderUnifier>();
        lmTileEdgeChecker = GetComponent<LMTileEdgeChecker>();
        lmSaver = GetComponent<LMSaver>();
        lmBasePrefabLimiter = GetComponent<LMBasePrefabLimiter>();
    }

    void Update()
    {
        foreach (var function in _updateFunctions)
            function();
    }

    void OnGUI()
    {
        foreach (var function in _onGUIFunctions)
            function();
    }

    public RaycastHit2D GetGridCellClicked()
    {
        var rayLength = 10f;
        var hit = Physics2D.GetRayIntersection(lmCamera.ScreenPointToRay(Input.mousePosition), rayLength, gridCollisionMask);

        return hit;
    }

    public List<GridCell> GetGridCellsHits(BasePrefab basePrefab)
    {
        var gridCells = new List<GridCell>();
        var hits = new List<Collider2D>();
        var prefabChildren = basePrefab.GetComponentsInChildren<BasePrefab>();

        foreach (var prefabChild in prefabChildren)
        {
            foreach (var collider in prefabChild.GetColliderHits(gridCollisionMask))
                hits.Add(collider);
        }
        if (hits.Count > 0)
        {
            for (int i = 0; i < hits.Count; i++)
                gridCells.Add(hits[i].gameObject.GetComponent<GridCell>());
        }

        return gridCells;
    }

    public void DeselectPlacedPrefab()
    {
        if (lmModifiableList.currentSelectedPrefab != null)
        {
            lmPrefabWaypointCreator.DeleteCurrentWaypoint();
            lmPrefabWaypointCreator.HideWaypoints();
            Delete(ref lmPrefabWaypointCreator.currentWaypoint);

            lmModifiableList.currentSelectedPrefab = null;
            lmModifiableList.DeleteAllModifableList();
        }
    }

    public void Delete<T>(ref T objectToDelete) where T : MonoBehaviour
    {
        if (objectToDelete != null)
        {
            objectToDelete.transform.position = new Vector2(1950, 1950);
            Destroy(objectToDelete.gameObject, 0.1f);
            objectToDelete = null;
        }
    }

    public void DeselectAndDeleteAll()
    {
        DeselectPlacedPrefab();
        Delete(ref lmDragAndDropOnGrid.currentDraggedPrefab);
        Delete(ref lmEraser.eraser);
        lmDragAndDropOnGrid.movingPlacedPrefab = false;
    }

    public void DeletePreviousPlayer(ref Player player)
    {
        var colliderHit = player.GetColliderHits(gridCollisionMask);
        var gridCellHit = colliderHit[0].GetComponent<GridCell>();
        Destroy(player.gameObject);
        gridCellHit.CheckGridPrefabFillment();
    }

    public bool IsCollidingWithUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        return false;
    }

    public void AddToUpdateFunctions(Action function)
    {
        if (_updateFunctions == null)
            _updateFunctions = new List<Action>();

        _updateFunctions.Add(function);
    }

    public void AddToOnGUIFunctions(Action function)
    {
        if (_onGUIFunctions == null)
            _onGUIFunctions = new List<Action>();

        _onGUIFunctions.Add(function);
    }

    public bool SelectedPrefabIsMobilePrefab()
    {
        var currentSelectedPrefab = lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();

        if (currentSelectedPrefab.hasWaypoints)
            return true;

        return false;
    }

    public void BackToMainMenu() // Llamado desde el botón...
    {
        StartCoroutine(ExecuteBackToMainMenu());
    }

    IEnumerator ExecuteBackToMainMenu()
    {
        CanvasUIManager.instance.EnableFade(CanvasUIManager.instance.diagonalLinesFade);

        yield return new WaitForSeconds(GameManager.fadeTime);

        SceneManager.LoadScene("Menu");
    }
}