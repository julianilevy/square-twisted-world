using UnityEngine;

public class BackgroundEye : TransformVector2
{
    public GameObject iris;
    public Animator eyelid;

    private Player _player;
    private Vector3 _centerPoint;
    private float _radius;

    void Start()
    {
        _centerPoint = iris.transform.localPosition;
        _radius = 0.5f;
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
        if (_player != null)
        {
            if (eyelid.GetCurrentAnimatorStateInfo(0).IsName("BackgroundEyelid_Open"))
            {
                var movement = iris.transform.position - _player.transform.position;
                var newPosition = iris.transform.localPosition - movement;
                var offset = newPosition - _centerPoint;
                var finalPos = _centerPoint + Vector3.ClampMagnitude(offset, _radius);
                iris.transform.localPosition = new Vector3(finalPos.x, finalPos.y, 0);
            }
        }
        else
        {
            if (eyelid.GetCurrentAnimatorStateInfo(0).IsName("BackgroundEyelid_Closed"))
                iris.transform.localPosition = _centerPoint;
        }
    }

    void CreateCollider()
    {
        if (GameManager.instance.IsPlayingLevel())
        {
            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 12;
            collider.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform.gameObject.layer == K.LAYER_PLAYER)
        {
            GetComponent<CircleCollider2D>().radius = 32;
            _player = collider2D.GetComponent<Player>();
            eyelid.GetComponent<Animator>().SetTrigger("OnCharacterSight");
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.transform.gameObject.layer == K.LAYER_PLAYER)
        {
            GetComponent<CircleCollider2D>().radius = 12;
            _player = null;
            eyelid.GetComponent<Animator>().SetTrigger("OnCharacterLost");
        }
    }
}