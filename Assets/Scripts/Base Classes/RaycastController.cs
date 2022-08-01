using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RaycastController : BasePrefab
{
    public LayerMask collisionMask;

    [HideInInspector]
    public RaycastOrigins raycastOrigins;
    [HideInInspector]
    public float skinWidth = 0.015f;
    [HideInInspector]
    public int verticalRayCount;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public float verticalRaySpacing;
    [HideInInspector]
    public float horizontalRaySpacing;

    private float _distanceBetweenRays = 0.25f;

    public override void Awake()
    {
        base.Awake();
        CalculateRaySpacing();
    }

    protected virtual void UpdateRayCastOrigins()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        bounds.Expand(skinWidth * -2);

        if (transform.rotation.eulerAngles == Rotation.EULER_UP)
        {
            raycastOrigins.topRight = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.bottomLeft = new Vector2(bounds.max.x, bounds.max.y);
        }
        else if (transform.rotation.eulerAngles == Rotation.EULER_RIGHT)
        {
            raycastOrigins.topRight = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.max.y);
            raycastOrigins.bottomLeft = new Vector2(bounds.max.x, bounds.min.y);
        }
        else if (transform.rotation.eulerAngles == Rotation.EULER_LEFT)
        {
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.max.x, bounds.max.y);
            raycastOrigins.bottomRight = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.max.y);
        }
        else if (transform.rotation.eulerAngles == Rotation.EULER_DOWN)
        {
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        }
    }

    protected void CalculateRaySpacing()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsWidth / _distanceBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsHeight / _distanceBetweenRays);

        horizontalRaySpacing = bounds.size.x / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.y / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topRight;
        public Vector2 topLeft;
        public Vector2 bottomRight;
        public Vector2 bottomLeft;
    }
}