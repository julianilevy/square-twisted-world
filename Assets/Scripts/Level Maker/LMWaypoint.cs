using UnityEngine;

public class LMWaypoint : TransformVector2
{
    [HideInInspector]
    public int index;
    [HideInInspector]
    public LMWaypoint previousWaypoint;

    private LineRenderer _lineRenderer;

    public override void Awake()
    {
        base.Awake();

        if (GameManager.instance.IsUsingLevelMaker())
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.sortingLayerName = "LM Helper";
        }
    }

    public override void Update()
    {
        base.Update();

        if (GameManager.instance.IsUsingLevelMaker())
            ConnectWaypoints();
    }

    void ConnectWaypoints()
    {
        if (previousWaypoint != null)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, previousWaypoint.transform.position);
        }
    }

    public void DeleteConnections()
    {
        previousWaypoint = null;
        _lineRenderer.startWidth = 0;
        _lineRenderer.endWidth = 0;
        _lineRenderer.positionCount = 0;
    }

    public bool IsCollidingAnotherWaypoint(LayerMask collisionMask)
    {
        var radius = GetComponent<CircleCollider2D>().radius;
        var hits = Physics2D.OverlapCircleAll(transform.position, radius, collisionMask);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.gameObject.GetComponent<LMWaypoint>())
                {
                    if (gameObject != hit.gameObject)
                        return true;
                }
            }
        }

        return false;
    }
}