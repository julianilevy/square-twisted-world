using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class MobileBase : RaycastController
{
    public Ref<float> speed;
    public Ref<float> waitTime;
    public Ref<float> startTime;
    public Ref<bool> cyclic;

    protected WaypointData[] savedWaypoints;
    protected Vector2 velocity;

    private Vector2[] _globalWaypoints;
    private float _percentBetweenWaypoints;
    private float _waitTimer;
    private float _startTimer;
    private bool _waitTimerOn;
    private int _fromWaypointIndex;
    private int _waypointListCountChecking;

    public override void Awake()
    {
        base.Awake();

        if (needsToBeCreated)
        {
            hasWaypoints = true;
            usableWaypointList = new List<LMWaypoint>();
            visualWaypointList = new List<Waypoint>();
            speed = new Ref<float>(20);
            waitTime = new Ref<float>(0.5f);
            startTime = new Ref<float>(0);
            cyclic = new Ref<bool>(false);
            _waitTimer = waitTime.value;
        }
    }

    public override void Update()
    {
        base.Update();
        UpdateGlobalWaypoints();

        if (GameManager.instance.IsPlayingLevel())
        {
            StartTimer();
            if (StartTimerIsOver())
            {
                UpdateRayCastOrigins();
                velocity = CalculatePlatformMovement();
                CalculateMovableObjectsMovement();
                MovePlatform();
                MoveMovableObject();
            }
        }
        else if (GameManager.instance.IsUsingLevelMaker())
            CheckWaypointsOnCyclic();
    }

    void UpdateGlobalWaypoints()
    {
        if (_waypointListCountChecking != usableWaypointList.Count)
        {
            _globalWaypoints = new Vector2[usableWaypointList.Count];
            for (int i = 0; i < usableWaypointList.Count; i++)
            {
                var waypointPosition = Utility.ToVector2(usableWaypointList[i].transform.position);
                _globalWaypoints[i] = waypointPosition + transformVector2.position;
            }

            _waypointListCountChecking = usableWaypointList.Count;
        }
    }

    void StartTimer()
    {
        if (startTime != null)
        {
            if (_startTimer < startTime.value)
                _startTimer += Time.deltaTime;
        }
    }

    Vector2 CalculatePlatformMovement()
    {
        if (_globalWaypoints != null)
        {
            if (_globalWaypoints.Length > 1)
            {
                if (!_waitTimerOn)
                {
                    _fromWaypointIndex %= _globalWaypoints.Length;
                    var toWayPointIndex = (_fromWaypointIndex + 1) % _globalWaypoints.Length;
                    var distanceBetweenWaypoints = Vector2.Distance(_globalWaypoints[_fromWaypointIndex], _globalWaypoints[toWayPointIndex]);
                    _percentBetweenWaypoints += speed.value / distanceBetweenWaypoints * Time.deltaTime;
                    _percentBetweenWaypoints = Mathf.Clamp01(_percentBetweenWaypoints);

                    var newPos = Vector2.Lerp(_globalWaypoints[_fromWaypointIndex], _globalWaypoints[toWayPointIndex], _percentBetweenWaypoints);

                    if (_percentBetweenWaypoints >= 1)
                    {
                        _percentBetweenWaypoints = 0;
                        _fromWaypointIndex++;

                        if (!cyclic.value)
                        {
                            if (_fromWaypointIndex >= _globalWaypoints.Length - 1)
                            {
                                _fromWaypointIndex = 0;
                                Array.Reverse(_globalWaypoints);
                            }
                        }

                        _waitTimerOn = true;
                    }
                    return newPos - transformVector2.position;
                }
            }
            if (_waitTimerOn)
            {
                _waitTimer -= Time.deltaTime;

                if (_waitTimer <= 0)
                {
                    _waitTimer = waitTime.value;
                    _waitTimerOn = false;
                }
            }
        }

        return Vector2.zero;
    }

    protected virtual void MoveMovableObject()
    {
    }

    protected virtual void CalculateMovableObjectsMovement()
    {
    }

    void MovePlatform()
    {
        transform.Translate(velocity);
    }

    protected bool StartTimerIsOver()
    {
        if (startTime != null)
        {
            if (_startTimer >= startTime.value)
                return true;
        }

        return false;
    }

    protected void SaveMobileBaseData<T>(ref T mobileBaseData) where T : MobileBaseData
    {
        savedWaypoints = new WaypointData[usableWaypointList.Count];

        for (int i = 0; i < usableWaypointList.Count; i++)
        {
            var savedWaypoint = new WaypointData();
            savedWaypoint.SetPositionValues(usableWaypointList[i].transform.position.x, usableWaypointList[i].transform.position.y, usableWaypointList[i].transform.position.z);
            savedWaypoint.SetRotationValues(usableWaypointList[i].transform.rotation.x, usableWaypointList[i].transform.rotation.y, usableWaypointList[i].transform.rotation.z, usableWaypointList[i].transform.rotation.w);
            savedWaypoint.SetIndexValue(usableWaypointList[i].index);
            savedWaypoints[i] = savedWaypoint;
        }

        mobileBaseData.SetMobileValues(speed.value, waitTime.value, startTime.value, cyclic.value);
    }

    protected void LoadMobileBaseData<T>(ref T mobileBaseData) where T : MobileBaseData
    {
        usableWaypointList = new List<LMWaypoint>();
        visualWaypointList = new List<Waypoint>();

        if (mobileBaseData.waypointData.Length > 0)
        {
            for (int i = 0; i < mobileBaseData.waypointData.Length; i++)
            {
                if (GameManager.instance.IsPlayingLevel())
                {
                    LoadUsableWaypointOnPlay(ref mobileBaseData, i);
                    LoadVisualWaypoint(ref mobileBaseData, i, false);
                }
                else if (GameManager.instance.IsEditingLevel())
                {
                    LoadUsableWaypointOnLevelMaker(ref mobileBaseData, i);
                    LoadVisualWaypoint(ref mobileBaseData, i, true);
                }
            }
        }

        UpdateGlobalWaypoints();
        speed = Ref.Create((float)mobileBaseData.speed);
        waitTime = Ref.Create((float)mobileBaseData.waitTime);
        startTime = Ref.Create((float)mobileBaseData.startTime);
        cyclic = Ref.Create(mobileBaseData.cyclic);
        _waitTimer = waitTime.value;
        CheckWaypointsOnCyclic();
    }

    void LoadUsableWaypointOnPlay<T>(ref T mobileBaseData, int i) where T : MobileBaseData
    {
        var newGameObject = new GameObject();
        newGameObject.name = "LM Waypoint";
        var newWaypoint = newGameObject.AddComponent<LMWaypoint>();

        newWaypoint.index = mobileBaseData.waypointData[i].index;
        newWaypoint.transform.position = new Vector3((float)mobileBaseData.waypointData[i].position[0], (float)mobileBaseData.waypointData[i].position[1], (float)mobileBaseData.waypointData[i].position[2]);
        newWaypoint.transform.rotation = new Quaternion((float)mobileBaseData.waypointData[i].rotation[0], (float)mobileBaseData.waypointData[i].rotation[1], (float)mobileBaseData.waypointData[i].rotation[2], (float)mobileBaseData.waypointData[i].rotation[3]);
        usableWaypointList.Add(newWaypoint);
    }

    void LoadUsableWaypointOnLevelMaker<T>(ref T mobileBaseData, int i) where T : MobileBaseData
    {
        var newGameObject = Instantiate(LevelMakerManager.instance.lmWaypointPicker.lmWaypoint);
        var newWaypoint = newGameObject.GetComponent<LMWaypoint>();
        newWaypoint.transform.SetParent(transform);

        newWaypoint.index = mobileBaseData.waypointData[i].index;
        newWaypoint.name = "[" + gameObject.name + "] " + "LM Waypoint " + newWaypoint.index;
        newWaypoint.transform.position = new Vector3((float)mobileBaseData.waypointData[i].position[0], (float)mobileBaseData.waypointData[i].position[1], (float)mobileBaseData.waypointData[i].position[2]);
        newWaypoint.transform.rotation = new Quaternion((float)mobileBaseData.waypointData[i].rotation[0], (float)mobileBaseData.waypointData[i].rotation[1], (float)mobileBaseData.waypointData[i].rotation[2], (float)mobileBaseData.waypointData[i].rotation[3]);

        if (usableWaypointList.Count > 0)
            newWaypoint.previousWaypoint = usableWaypointList[usableWaypointList.Count - 1];
        usableWaypointList.Add(newWaypoint);

        newWaypoint.gameObject.SetActive(false);
    }

    void LoadVisualWaypoint<T>(ref T mobileBaseData, int i, bool isOnLevelMaker) where T : MobileBaseData
    {
        var newGameObject = Instantiate(LevelMakerManager.instance.lmWaypointPicker.waypoint);
        var newWaypoint = newGameObject.GetComponent<Waypoint>();
        if (isOnLevelMaker)
            newWaypoint.transform.SetParent(transform);

        newWaypoint.index = mobileBaseData.waypointData[i].index;
        newWaypoint.ableToSetSortingOrder = true;
        newWaypoint.name = "[" + gameObject.name + "] " + "Waypoint " + newWaypoint.index;
        newWaypoint.transform.position = new Vector3((float)mobileBaseData.waypointData[i].position[0], (float)mobileBaseData.waypointData[i].position[1], (float)mobileBaseData.waypointData[i].position[2]);
        newWaypoint.transform.rotation = new Quaternion((float)mobileBaseData.waypointData[i].rotation[0], (float)mobileBaseData.waypointData[i].rotation[1], (float)mobileBaseData.waypointData[i].rotation[2], (float)mobileBaseData.waypointData[i].rotation[3]);

        if (visualWaypointList.Count > 0)
        {
            newWaypoint.previousWaypoint = visualWaypointList[visualWaypointList.Count - 1];
            visualWaypointList[visualWaypointList.Count - 1].nextWaypoint = newWaypoint;
        }
        visualWaypointList.Add(newWaypoint);
    }

    void CheckWaypointsOnCyclic()
    {
        if (cyclic.value)
        {
            if (visualWaypointList.Count > 2)
            {
                var firstWaypoint = visualWaypointList[0];
                var lastWaypoint = visualWaypointList[visualWaypointList.Count - 1];

                firstWaypoint.previousWaypoint = lastWaypoint;
                firstWaypoint.EnableConnections(true);
                lastWaypoint.nextWaypoint = lastWaypoint;
            }
        }
        else
        {
            if (visualWaypointList.Count > 2)
            {
                var firstWaypoint = visualWaypointList[0];
                var lastWaypoint = visualWaypointList[visualWaypointList.Count - 1];

                firstWaypoint.previousWaypoint = null;
                firstWaypoint.EnableConnections(false);
                lastWaypoint.nextWaypoint = null;
            }
        }

        if (visualWaypointList.Count > 0 && visualWaypointList.Count <= 2)
        {
            var firstWaypoint = visualWaypointList[0];
            var lastWaypoint = visualWaypointList[visualWaypointList.Count - 1];

            firstWaypoint.previousWaypoint = null;
            firstWaypoint.EnableConnections(false);
            lastWaypoint.nextWaypoint = null;
        }
    }
}