using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LMPrefabWaypointCreator : MonoBehaviour
{
    [HideInInspector]
    public LMWaypoint currentWaypoint;
    [HideInInspector]
    public bool ableToDeselectCurrentPrefab = true;

    private LMSettings _lmSettings;
    private Waypoint _visualCurrentWaypoint;
    private Vector3 _currentGridPosition;
    private bool _waypointEditorMode;
    private bool _movingWaypoint;
    private bool _ableToCreateOrPlaceAnotherWaypoint;

    void Awake()
    {
        _lmSettings = GetComponent<LMSettings>();
        _lmSettings.AddToOnGUIFunctions(ShowOrHideWaypoints);
        _lmSettings.AddToOnGUIFunctions(CheckIfAbleToDeselectCurrentPrefab);
        _lmSettings.AddToOnGUIFunctions(DetectCurrentGridCell);
        _lmSettings.AddToOnGUIFunctions(CreateWaypoint);
        _lmSettings.AddToOnGUIFunctions(MoveCurrentWaypoint);
        _lmSettings.AddToOnGUIFunctions(CheckWaypointListCleaning);
        _lmSettings.AddToUpdateFunctions(PlaceCurrentWaypoint);
        _lmSettings.AddToUpdateFunctions(PickUpWaypoint);
        _lmSettings.AddToUpdateFunctions(DeleteWaypoint);
        _lmSettings.AddToUpdateFunctions(QuitWaypointEditorMode);
    }

    void ShowOrHideWaypoints()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (!_waypointEditorMode)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                ShowWaypoints();
            else
                HideWaypoints();
        }
    }

    void CheckIfAbleToDeselectCurrentPrefab()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (currentWaypoint == null)
            ableToDeselectCurrentPrefab = true;
        else
            ableToDeselectCurrentPrefab = false;
    }

    void DetectCurrentGridCell()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        var hit = _lmSettings.GetGridCellClicked();

        if (hit)
        {
            if (_currentGridPosition != hit.transform.position)
                _currentGridPosition = hit.transform.position;
        }
    }

    void CreateWaypoint()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            if (!_lmSettings.IsCollidingWithUI())
            {
                if (currentWaypoint == null)
                {
                    var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();

                    SetNewCurrentWaypoint();
                    if (currentSelectedPrefab.usableWaypointList.Count == 0)
                        CreateNewWaypoint(currentSelectedPrefab.transform.position);

                    SetNewCurrentVisualWaypoint();
                    if (currentSelectedPrefab.visualWaypointList.Count == 0)
                        CreateNewVisualWaypoint(currentSelectedPrefab.transform.position);

                    StartCoroutine(WaypointCreationOrPlacingCooldown());
                }
            }
        }
    }

    void SetNewCurrentWaypoint()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        var newCurrentWaypoint = Instantiate(LevelMakerManager.instance.lmWaypointPicker.lmWaypoint.gameObject);
        newCurrentWaypoint.transform.position = _currentGridPosition;
        newCurrentWaypoint.name = "LMWaypoint";
        currentWaypoint = newCurrentWaypoint.GetComponent<LMWaypoint>();

        var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();
        if (currentSelectedPrefab.usableWaypointList.Count > 0)
            currentWaypoint.previousWaypoint = currentSelectedPrefab.usableWaypointList[currentSelectedPrefab.usableWaypointList.Count - 1];
    }

    void CreateNewWaypoint(Vector3 position)
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        var newWaypointGO = Instantiate(LevelMakerManager.instance.lmWaypointPicker.lmWaypoint.gameObject);
        newWaypointGO.transform.position = position;
        newWaypointGO.transform.SetParent(_lmSettings.lmModifiableList.currentSelectedPrefab.transform);

        var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();
        var waypoint = newWaypointGO.GetComponent<LMWaypoint>();
        waypoint.index = currentSelectedPrefab.usableWaypointList.Count;
        waypoint.gameObject.name = "[" + currentSelectedPrefab.gameObject.name + "] " + "LMWaypoint " + waypoint.index;

        if (currentSelectedPrefab.usableWaypointList.Count > 0)
            waypoint.previousWaypoint = currentSelectedPrefab.usableWaypointList[waypoint.index - 1];
        currentSelectedPrefab.usableWaypointList.Add(waypoint);

        if (!_movingWaypoint)
            currentWaypoint.previousWaypoint = waypoint;
    }

    void SetNewCurrentVisualWaypoint()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        var newVisualCurrentWaypoint = Instantiate(LevelMakerManager.instance.lmWaypointPicker.waypoint.gameObject);
        newVisualCurrentWaypoint.transform.position = _currentGridPosition;
        newVisualCurrentWaypoint.name = "Waypoint";
        _visualCurrentWaypoint = newVisualCurrentWaypoint.GetComponent<Waypoint>();

        var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();
        if (currentSelectedPrefab.visualWaypointList.Count > 0)
        {
            _visualCurrentWaypoint.previousWaypoint = currentSelectedPrefab.visualWaypointList[currentSelectedPrefab.visualWaypointList.Count - 1];
            currentSelectedPrefab.visualWaypointList[currentSelectedPrefab.visualWaypointList.Count - 1].nextWaypoint = _visualCurrentWaypoint;
        }
    }

    void CreateNewVisualWaypoint(Vector3 position)
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        var newWaypointGO = Instantiate(LevelMakerManager.instance.lmWaypointPicker.waypoint.gameObject);
        newWaypointGO.transform.position = position;
        newWaypointGO.transform.SetParent(_lmSettings.lmModifiableList.currentSelectedPrefab.transform);

        var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();
        var waypoint = newWaypointGO.GetComponent<Waypoint>();
        waypoint.ableToSetSortingOrder = true;
        waypoint.index = currentSelectedPrefab.visualWaypointList.Count;
        waypoint.gameObject.name = "[" + currentSelectedPrefab.gameObject.name + "] " + "Waypoint " + waypoint.index;

        if (currentSelectedPrefab.visualWaypointList.Count > 0)
        {
            waypoint.previousWaypoint = currentSelectedPrefab.visualWaypointList[waypoint.index - 1];
            currentSelectedPrefab.visualWaypointList[waypoint.index - 1].nextWaypoint = waypoint;
        }
        currentSelectedPrefab.visualWaypointList.Add(waypoint);

        if (!_movingWaypoint)
        {
            _visualCurrentWaypoint.previousWaypoint = waypoint;
            waypoint.nextWaypoint = _visualCurrentWaypoint;
        }
    }

    IEnumerator WaypointCreationOrPlacingCooldown()
    {
        _waypointEditorMode = true;
        _ableToCreateOrPlaceAnotherWaypoint = false;
        yield return new WaitForSeconds(0.1f);
        _ableToCreateOrPlaceAnotherWaypoint = true;
    }

    void PlaceCurrentWaypoint()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!_lmSettings.IsCollidingWithUI())
            {
                if (currentWaypoint != null)
                {
                    if (_ableToCreateOrPlaceAnotherWaypoint)
                    {
                        if (!currentWaypoint.IsCollidingAnotherWaypoint(_lmSettings.waypointsCollisionMask))
                        {
                            if (!_movingWaypoint)
                            {
                                CreateNewWaypoint(_currentGridPosition);
                                CreateNewVisualWaypoint(_currentGridPosition);
                            }
                            else
                            {
                                currentWaypoint = null;
                                _movingWaypoint = false;
                            }

                            StartCoroutine(WaypointCreationOrPlacingCooldown());
                        }
                    }
                }
            }
        }
    }

    void PickUpWaypoint()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!_lmSettings.IsCollidingWithUI())
            {
                if (currentWaypoint == null)
                {
                    if (_ableToCreateOrPlaceAnotherWaypoint)
                    {
                        var rayLength = 10f;
                        var hits = Physics2D.GetRayIntersectionAll(_lmSettings.lmCamera.ScreenPointToRay(Input.mousePosition), rayLength, _lmSettings.waypointsCollisionMask);

                        foreach (var hit in hits)
                        {
                            if (hit)
                            {
                                if (hit.collider.gameObject.GetComponent<LMWaypoint>())
                                {
                                    var hitWaypoint = hit.transform.GetComponent<LMWaypoint>();
                                    if (hitWaypoint.index > 0)
                                    {
                                        currentWaypoint = hitWaypoint;
                                        _movingWaypoint = true;
                                    }
                                }
                                if (hit.collider.gameObject.GetComponent<Waypoint>())
                                {
                                    var hitVisualWaypoint = hit.transform.GetComponent<Waypoint>();
                                    if (hitVisualWaypoint.index > 0)
                                        _visualCurrentWaypoint = hitVisualWaypoint;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void MoveCurrentWaypoint()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (currentWaypoint != null)
        {
            currentWaypoint.transform.position = _currentGridPosition;
            if (_visualCurrentWaypoint != null)
                _visualCurrentWaypoint.transform.position = _currentGridPosition;
        }
    }

    void DeleteWaypoint()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (Input.GetMouseButtonDown(1))
            DeleteCurrentWaypoint();
    }

    public void DeleteCurrentWaypoint()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (currentWaypoint != null)
        {
            if (_movingWaypoint)
            {
                var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();
                var waypointsToDelete = new List<LMWaypoint>();
                var visualWaypointsToDelete = new List<Waypoint>();

                while (currentSelectedPrefab.usableWaypointList.Count > currentWaypoint.index)
                {
                    waypointsToDelete.Add(currentSelectedPrefab.usableWaypointList[currentSelectedPrefab.usableWaypointList.Count - 1]);
                    currentSelectedPrefab.usableWaypointList.RemoveAt(currentSelectedPrefab.usableWaypointList.Count - 1);
                    visualWaypointsToDelete.Add(currentSelectedPrefab.visualWaypointList[currentSelectedPrefab.visualWaypointList.Count - 1]);
                    currentSelectedPrefab.visualWaypointList.RemoveAt(currentSelectedPrefab.visualWaypointList.Count - 1);
                }
                foreach (var waypoint in waypointsToDelete)
                    Destroy(waypoint.gameObject);
                foreach (var visualWaypoint in visualWaypointsToDelete)
                    Destroy(visualWaypoint.gameObject);

                currentWaypoint = null;
                _visualCurrentWaypoint = null;
                _movingWaypoint = false;
            }
            else
            {
                currentWaypoint.DeleteConnections();
                _lmSettings.Delete(ref currentWaypoint);
                _visualCurrentWaypoint.DeleteConnections();
                _lmSettings.Delete(ref _visualCurrentWaypoint);
            }
        }
    }

    void CheckWaypointListCleaning()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (currentWaypoint != null)
        {
            if (_movingWaypoint)
            {
                var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();
                if (currentSelectedPrefab.usableWaypointList.Count <= 1)
                {
                    foreach (var waypoint in currentSelectedPrefab.usableWaypointList)
                        Destroy(waypoint.gameObject);
                    foreach (var visualWaypoint in currentSelectedPrefab.visualWaypointList)
                        Destroy(visualWaypoint.gameObject);
                    currentSelectedPrefab.usableWaypointList.Clear();
                    currentSelectedPrefab.visualWaypointList.Clear();
                    currentWaypoint = null;
                    _visualCurrentWaypoint = null;
                    _movingWaypoint = false;
                }
            }
        }
    }

    void ShowWaypoints()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();

        if (currentSelectedPrefab.hasWaypoints)
        {
            foreach (var waypoint in currentSelectedPrefab.usableWaypointList)
                waypoint.gameObject.SetActive(true);
        }
    }

    public void HideWaypoints()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        var currentSelectedPrefab = _lmSettings.lmModifiableList.currentSelectedPrefab.GetComponent<BasePrefab>();

        if (currentSelectedPrefab.hasWaypoints)
        {
            foreach (var waypoint in currentSelectedPrefab.usableWaypointList)
                waypoint.gameObject.SetActive(false);
        }

        _waypointEditorMode = false;
    }

    void QuitWaypointEditorMode()
    {
        if (!IsCurrentMobilePrefabSelected())
            return;

        if (_waypointEditorMode)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                HideWaypoints();
        }
    }

    bool IsCurrentMobilePrefabSelected()
    {
        if (_lmSettings.lmModifiableList.currentSelectedPrefab != null)
        {
            if (_lmSettings.SelectedPrefabIsMobilePrefab())
                return true;
        }
        else
            return false;

        return false;
    }

    public bool IsWaypointEditorModeOn()
    {
        return _waypointEditorMode;
    }
}