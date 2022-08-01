using UnityEngine;

public class EyeIris : TransformVector2
{
    public float radius = 0.75f;
    public bool moveWithParent = true;

    private Player _player;
    private Vector2 _centerPoint;

    void Start()
    {
        if (moveWithParent)
            _centerPoint = transform.localPosition;
        else
            _centerPoint = transform.position;

        CreateCollider();
    }

    public override void Update()
    {
        base.Update();
        if (GameManager.instance.IsPlayingLevel())
            Move();
    }

    void Move()
    {
        if (moveWithParent)
        {
            if (_player != null)
            {
                Vector2 movement = transform.position - _player.transform.position;
                Vector2 newPosition = transformVector2.localPosition - movement;
                Vector2 offset = newPosition - _centerPoint;

                transform.localPosition = _centerPoint + Vector2.ClampMagnitude(offset, radius);
            }
            else
                transform.localPosition = _centerPoint;
        }
        else
        {
            if (_player != null)
            {
                Vector2 movement = transform.position - _player.transform.position;
                Vector2 newPosition = new Vector2(transform.position.x, transform.position.y) - movement;
                Vector2 offset = newPosition - _centerPoint;

                transform.position = _centerPoint + Vector2.ClampMagnitude(offset, radius);
            }
            else
                transform.position = _centerPoint;
        }
    }

    void CreateCollider()
    {
        if (GameManager.instance.IsPlayingLevel())
        {
            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 30;
            collider.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform.gameObject.layer == K.LAYER_PLAYER)
            _player = collider2D.GetComponent<Player>();
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.transform.gameObject.layer == K.LAYER_PLAYER)
            _player = null;
    }
}