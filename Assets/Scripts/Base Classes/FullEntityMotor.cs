using UnityEngine;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

public class FullEntityMotor : EntityMotor
{
    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    protected bool grabbedByPlayer;

    private float _maxGravityValue = -128.3951f;
    private bool _justFell = true;

    public override void Awake()
    {
        base.Awake();
        gravityValue = _maxGravityValue;
    }

    public override void Update()
    {
        base.Update();
        if (GameManager.instance.IsPlayingLevel())
        {
            if (!locked)
            {
                CalculateVelocity();
                HandleMovement();
                HandleVelocityAtFloor();
            }
        }
    }

    void CalculateVelocity()
    {
        velocity.y += gravityValue * Time.deltaTime;
    }

    void HandleMovement()
    {
        Move(velocity * Time.deltaTime);
    }

    void HandleVelocityAtFloor()
    {
        if (collisions.up || collisions.down)
            velocity.y = 0;

        if (collisions.down)
        {
            if (!_justFell)
            {
                if (GetComponent<Key>())
                    MasterAudio.PlaySound3DFollowTransformAndForget("KeyTouchesGround", transform);
            }
            _justFell = true;
        }
        else
            _justFell = false;
    }

    public void GetGrabbedByPlayer()
    {
        foreach (var sprite in sprites)
            sprite.enabled = false;
        rotating = false;
        locked = false;
        pushedByGravityPlatform = false;
        grabbedByPlayer = true;
        gravityValue = 0;
        gameObject.layer = K.LAYER_DEFAULT;
    }

    public void GetReleasedFromPlayer()
    {
        foreach (var sprite in sprites)
            sprite.enabled = true;
        EndRotation(transform.rotation);
        grabbedByPlayer = false;
        gravityValue = _maxGravityValue;
        gameObject.layer = K.LAYER_GRAVITYENTITY;
    }
}