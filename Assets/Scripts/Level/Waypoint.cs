using UnityEngine;
using System.Collections;

public class Waypoint : TransformVector2
{
    [HideInInspector]
    public int index;
    [HideInInspector]
    public bool ableToSetSortingOrder;
    [HideInInspector]
    public Waypoint previousWaypoint;
    [HideInInspector]
    public Waypoint nextWaypoint;
    [HideInInspector]
    public SpriteRenderer middleConnector;
    [HideInInspector]
    public SpriteRenderer middleConnectorEmission;
    [HideInInspector]
    public SpriteRenderer endConnector;
    [HideInInspector]
    public SpriteRenderer endConnectorEmission;
    [HideInInspector]
    public LineRenderer lineRenderer;
    [HideInInspector]
    public LineRenderer lineRendererEmission;

    public override void Awake()
    {
        base.Awake();

        lineRenderer.sortingLayerName = "Lower Background";
        lineRendererEmission.sortingLayerName = "Lower Background";
        StartCoroutine(InitialSettings());
    }

    public override void Update()
    {
        base.Update();

        if (GameManager.instance.IsUsingLevelMaker())
        {
            ConnectWaypoints();
            UpdateWaypointSprites();
        }
    }

    void UpdateWaypointSprites()
    {
        if (previousWaypoint != null && nextWaypoint != null)
        {
            middleConnector.gameObject.SetActive(true);
            middleConnectorEmission.gameObject.SetActive(true);
            endConnector.gameObject.SetActive(false);
            endConnectorEmission.gameObject.SetActive(false);
        }
        else if (previousWaypoint == null && nextWaypoint == null)
        {
            middleConnector.gameObject.SetActive(false);
            middleConnectorEmission.gameObject.SetActive(false);
            endConnector.gameObject.SetActive(false);
            endConnectorEmission.gameObject.SetActive(false);
        }
        else
        {
            endConnector.gameObject.SetActive(true);
            endConnectorEmission.gameObject.SetActive(true);
            middleConnector.gameObject.SetActive(false);
            middleConnectorEmission.gameObject.SetActive(false);
        }
    }

    void ConnectWaypoints()
    {
        if (previousWaypoint != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, previousWaypoint.transform.position);
            lineRendererEmission.positionCount = 2;
            lineRendererEmission.SetPosition(0, transform.position);
            lineRendererEmission.SetPosition(1, previousWaypoint.transform.position);
        }
    }

    public void DeleteConnections()
    {
        previousWaypoint = null;
        lineRenderer.startWidth = 0;
        lineRenderer.endWidth = 0;
        lineRenderer.positionCount = 0;
        lineRendererEmission.startWidth = 0;
        lineRendererEmission.endWidth = 0;
        lineRendererEmission.positionCount = 0;
    }

    public void EnableConnections(bool value)
    {
        lineRenderer.gameObject.SetActive(value);
        lineRendererEmission.gameObject.SetActive(value);
    }

    #region More Efficient Line Renderers (Unused)

    /*void ConnectWaypoints()
    {
        if (index == 0)
        {
            var allNextWaypoints = GetAllNextWaypoints(this);

            lineRenderer.positionCount = allNextWaypoints.Count;
            lineRendererEmission.positionCount = allNextWaypoints.Count;

            for (int i = 0; i < allNextWaypoints.Count - 1; i++)
            {
                if (allNextWaypoints[i + 1].index > 0)
                {
                    allNextWaypoints[i + 1].lineRenderer.gameObject.SetActive(false);
                    allNextWaypoints[i + 1].lineRendererEmission.gameObject.SetActive(false);
                }

                lineRenderer.SetPosition(i, allNextWaypoints[i].transform.position);
                lineRenderer.SetPosition(i + 1, allNextWaypoints[i + 1].transform.position);
                lineRendererEmission.SetPosition(i, allNextWaypoints[i].transform.position);
                lineRendererEmission.SetPosition(i + 1, allNextWaypoints[i + 1].transform.position);
            }
        }
    }

    public List<Waypoint> GetAllNextWaypoints(Waypoint currentWaypoint, List<Waypoint> addedWaypoints = null)
    {
        if (addedWaypoints == null)
            addedWaypoints = new List<Waypoint>();

        addedWaypoints.Add(currentWaypoint);

        if (currentWaypoint.nextWaypoint != null)
        {
            if (!addedWaypoints.Contains(currentWaypoint.nextWaypoint))
                GetAllNextWaypoints(currentWaypoint.nextWaypoint, addedWaypoints);
            else
                addedWaypoints.Add(addedWaypoints[0]);
        }

        return addedWaypoints;
    }

    public void DeleteConnections()
    {
        previousWaypoint = null;
    }*/

    #endregion

    void SetSortingOrder()
    {
        if (LevelMakerManager.instance.lmWaypointPicker.sortingOrder <= 32760)
        {
            if (ableToSetSortingOrder)
            {
                lineRenderer.sortingOrder = LevelMakerManager.instance.lmWaypointPicker.sortingOrder++;
                lineRendererEmission.sortingOrder = LevelMakerManager.instance.lmWaypointPicker.sortingOrder++;
            }
        }
    }

    IEnumerator InitialSettings()
    {
        yield return new WaitForEndOfFrame();
        SetSortingOrder();
        ConnectWaypoints();
        UpdateWaypointSprites();
    }
}