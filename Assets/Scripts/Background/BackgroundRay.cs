using UnityEngine;

public class BackgroundRay : MonoBehaviour
{
    public LayerMask collisionMask;
    public BackgroundRayDirection direction;

    [HideInInspector]
    public GameObject baseCenter;
    [HideInInspector]
    public LineRenderer shooter;
    [HideInInspector]
    public Transform raySpawnPoint;

    private int _damage = 9999;
    private Vector3 _direction;

    public enum BackgroundRayDirection
    {
        Up,
        Right,
        Left,
        Down
    }

    void Awake()
    {
        shooter.sortingLayerName = "LM Helper";

        if (direction == BackgroundRayDirection.Up)
            _direction = baseCenter.transform.up;
        else if (direction == BackgroundRayDirection.Right)
            _direction = baseCenter.transform.right;
        else if (direction == BackgroundRayDirection.Left)
            _direction = -baseCenter.transform.right;
        else if (direction == BackgroundRayDirection.Down)
            _direction = -baseCenter.transform.up;
    }

    public void Update()
    {
        CheckCollision();
    }

    void CheckCollision()
    {
        var hit = Physics2D.Raycast(raySpawnPoint.position, _direction, Mathf.Infinity, collisionMask);
        if (hit)
        {
            shooter.positionCount = 2;
            shooter.SetPosition(0, raySpawnPoint.position);
            shooter.SetPosition(1, hit.point);

            if (hit.transform.gameObject.layer == K.LAYER_PLAYER)
            {
                var player = hit.transform.GetComponent<Player>();
                player.GetDamage(_damage);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform.gameObject.layer == K.LAYER_PLAYER)
        {
            var player = collider2D.GetComponent<Player>();
            player.GetDamage(_damage);
        }
    }
}