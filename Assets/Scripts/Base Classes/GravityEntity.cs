using DarkTonic.MasterAudio;
using UnityEngine;

public class GravityEntity : RaycastController
{
    [HideInInspector]
    public Vector2 velocity;
    [HideInInspector]
    public float gravityValue;
    [HideInInspector]
    public bool locked;
    [HideInInspector]
    public bool pushedByGravityPlatform;
    [HideInInspector]
    public bool rotating;
    [HideInInspector]
    public Vector2 previousGravity;
    [HideInInspector]
    public Vector2 currentGravity;

    private Quaternion _originalRotation;
    private Vector3 _beforeRotationPosition;
    private string _touchedPlatformSide;
    private float _rotatingProgress;
    private float _rotatingSpeed = 0.8f;
    private bool _originalRotationSet;
    private bool _beforeRotationPositionSet;
    private bool _waitingToSetGravity;

    public override void Awake()
    {
        base.Awake();
        EqualRotationToGravity();
        canBeRotated = true;
    }

    public override void Update()
    {
        base.Update();

        if (GameManager.instance.IsPlayingLevel())
        {
            ProvisionalGravity();
            WaitToSetGravity();
            GravityChangeRotate();
        }
    }

    public void SetNewGravity(Vector2 newGravity, string touchedPlatform)
    {
        velocity = Vector2.zero;
        previousGravity = currentGravity;
        currentGravity = newGravity;
        _touchedPlatformSide = touchedPlatform;
    }

    public void PushEntity(float pushForce, Vector2 direction, bool pushedManually = false)
    {
        if (previousGravity == Gravity.UP || previousGravity == Gravity.DOWN)
            velocity = transform.TransformDirection(direction) * pushForce;
        else if (previousGravity == Gravity.RIGHT || previousGravity == Gravity.LEFT)
            velocity = transform.TransformDirection(-direction) * pushForce;

        locked = true;
        pushedByGravityPlatform = true;
        _waitingToSetGravity = true;

        if (GetComponent<Player>())
            MasterAudio.PlaySound3DFollowTransformAndForget("GravityPlatformJump", transform);

        OnEntityPushed();
    }

    protected virtual void OnEntityPushed()
    {
    }

    void ProvisionalGravity()
    {
        if (pushedByGravityPlatform)
        {
            if (previousGravity == Gravity.UP)
            {
                if (_touchedPlatformSide == K.SIDE_UP)
                    velocity.y += gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_RIGHT)
                    velocity.x += gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_LEFT)
                    velocity.x -= gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_DOWN)
                    velocity.y -= gravityValue * Time.deltaTime;

                transform.Translate(velocity * Time.deltaTime);
            }
            else if (previousGravity == Gravity.RIGHT)
            {
                if (_touchedPlatformSide == K.SIDE_UP)
                    velocity.x -= gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_RIGHT)
                    velocity.y += gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_LEFT)
                    velocity.y -= gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_DOWN)
                    velocity.x += gravityValue * Time.deltaTime;

                transform.Translate(velocity * Time.deltaTime);
            }
            else if (previousGravity == Gravity.LEFT)
            {
                if (_touchedPlatformSide == K.SIDE_UP)
                    velocity.x += gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_RIGHT)
                    velocity.y -= gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_LEFT)
                    velocity.y += gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_DOWN)
                    velocity.x -= gravityValue * Time.deltaTime;

                transform.Translate(velocity * Time.deltaTime);
            }
            else if (previousGravity == Gravity.DOWN)
            {
                if (_touchedPlatformSide == K.SIDE_UP)
                    velocity.y -= gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_RIGHT)
                    velocity.x -= gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_LEFT)
                    velocity.x += gravityValue * Time.deltaTime;
                else if (_touchedPlatformSide == K.SIDE_DOWN)
                    velocity.y += gravityValue * Time.deltaTime;

                transform.Translate(velocity * Time.deltaTime);
            }
        }
    }

    void WaitToSetGravity()
    {
        if (_waitingToSetGravity)
        {
            if (previousGravity == Gravity.UP)
            {
                if (_touchedPlatformSide == K.SIDE_UP)
                {
                    var hitObstacle = CheckCollisionsAfterPush(transform.up, ref horizontalRayCount, ref raycastOrigins.topLeft);
                    if (hitObstacle || velocity.y <= -0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_RIGHT)
                {
                    var hitObstacle = CheckCollisionsAfterPush(transform.right, ref verticalRayCount, ref raycastOrigins.bottomRight);
                    if (hitObstacle || velocity.x <= -0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_LEFT)
                {
                    var hitObstacle = CheckCollisionsAfterPush(-transform.right, ref verticalRayCount, ref raycastOrigins.bottomLeft);
                    if (hitObstacle || velocity.x >= 0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_DOWN)
                {
                    var hitObstacle = CheckCollisionsAfterPush(-transform.up, ref horizontalRayCount, ref raycastOrigins.bottomLeft);
                    if (hitObstacle || velocity.y >= 0.1f)
                        SetGravityAfterWaiting();
                }
            }
            else if (previousGravity == Gravity.RIGHT)
            {
                if (_touchedPlatformSide == K.SIDE_UP)
                {
                    var hitObstacle = CheckCollisionsAfterPush(-transform.right, ref verticalRayCount, ref raycastOrigins.bottomLeft);
                    if (hitObstacle || velocity.x >= 0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_RIGHT)
                {
                    var hitObstacle = CheckCollisionsAfterPush(transform.up, ref horizontalRayCount, ref raycastOrigins.topLeft);
                    if (hitObstacle || velocity.y <= -0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_LEFT)
                {
                    var hitObstacle = CheckCollisionsAfterPush(-transform.up, ref horizontalRayCount, ref raycastOrigins.bottomLeft);
                    if (hitObstacle || velocity.y >= 0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_DOWN)
                {
                    var hitObstacle = CheckCollisionsAfterPush(transform.right, ref verticalRayCount, ref raycastOrigins.bottomRight);
                    if (hitObstacle || velocity.x <= -0.1f)
                        SetGravityAfterWaiting();
                }
            }
            else if (previousGravity == Gravity.LEFT)
            {
                if (_touchedPlatformSide == K.SIDE_UP)
                {
                    var hitObstacle = CheckCollisionsAfterPush(transform.right, ref verticalRayCount, ref raycastOrigins.bottomRight);
                    if (hitObstacle || velocity.x <= -0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_RIGHT)
                {
                    var hitObstacle = CheckCollisionsAfterPush(-transform.up, ref horizontalRayCount, ref raycastOrigins.bottomLeft);
                    if (hitObstacle || velocity.y >= 0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_LEFT)
                {
                    var hitObstacle = CheckCollisionsAfterPush(transform.up, ref horizontalRayCount, ref raycastOrigins.topLeft);
                    if (hitObstacle || velocity.y <= -0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_DOWN)
                {
                    var hitObstacle = CheckCollisionsAfterPush(-transform.right, ref verticalRayCount, ref raycastOrigins.bottomLeft);
                    if (hitObstacle || velocity.x >= 0.1f)
                        SetGravityAfterWaiting();
                }
            }
            else if (previousGravity == Gravity.DOWN)
            {
                if (_touchedPlatformSide == K.SIDE_UP)
                {
                    var hitObstacle = CheckCollisionsAfterPush(-transform.up, ref horizontalRayCount, ref raycastOrigins.bottomLeft);
                    if (hitObstacle || velocity.y >= 0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_RIGHT)
                {
                    var hitObstacle = CheckCollisionsAfterPush(-transform.right, ref verticalRayCount, ref raycastOrigins.bottomLeft);
                    if (hitObstacle || velocity.x >= 0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_LEFT)
                {
                    var hitObstacle = CheckCollisionsAfterPush(transform.right, ref verticalRayCount, ref raycastOrigins.bottomRight);
                    if (hitObstacle || velocity.x <= -0.1f)
                        SetGravityAfterWaiting();
                }
                else if (_touchedPlatformSide == K.SIDE_DOWN)
                {
                    var hitObstacle = CheckCollisionsAfterPush(transform.up, ref horizontalRayCount, ref raycastOrigins.topLeft);
                    if (hitObstacle || velocity.y <= -0.1f)
                        SetGravityAfterWaiting();
                }
            }
        }
    }

    public void SetGravityAfterWaiting()
    {
        velocity = Vector2.zero;
        pushedByGravityPlatform = false;
        rotating = true;
        _waitingToSetGravity = false;

        if (GetComponent<Player>())
            MasterAudio.PlaySound3DFollowTransformAndForget("GravityChange", transform);
    }

    void GravityChangeRotate()
    {
        if (rotating)
        {
            if (previousGravity == Gravity.UP)
            {
                if (_touchedPlatformSide == K.SIDE_UP || _touchedPlatformSide == K.SIDE_DOWN)
                {
                    if (currentGravity == Gravity.RIGHT)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.RIGHT02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.RIGHT02);
                    }
                    if (currentGravity == Gravity.LEFT)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.LEFT01, _rotatingSpeed);
                        else
                            EndRotation(Rotation.LEFT01);
                    }
                    if (currentGravity == Gravity.DOWN)
                    {
                        if (_touchedPlatformSide == K.SIDE_UP)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.DOWN02, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.DOWN02);
                        }
                        else if (_touchedPlatformSide == K.SIDE_DOWN)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.DOWN01, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.DOWN01);
                        }
                    }
                }
                if (_touchedPlatformSide == K.SIDE_RIGHT || _touchedPlatformSide == K.SIDE_LEFT)
                {
                    if (currentGravity == Gravity.RIGHT)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.RIGHT02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.RIGHT02);
                    }
                    if (currentGravity == Gravity.LEFT)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.LEFT01, _rotatingSpeed);
                        else
                            EndRotation(Rotation.LEFT01);
                    }
                    if (currentGravity == Gravity.DOWN)
                    {
                        if (_touchedPlatformSide == K.SIDE_RIGHT)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.DOWN01, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.DOWN01);
                        }
                        else if (_touchedPlatformSide == K.SIDE_LEFT)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.DOWN02, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.DOWN02);
                        }
                    }
                }
            }
            if (previousGravity == Gravity.RIGHT)
            {
                if (_touchedPlatformSide == K.SIDE_UP || _touchedPlatformSide == K.SIDE_DOWN)
                {
                    if (currentGravity == Gravity.UP)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.UP02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.UP02);
                    }
                    if (currentGravity == Gravity.LEFT)
                    {
                        if (_touchedPlatformSide == K.SIDE_UP)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.LEFT01, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.LEFT01);
                        }
                        else if (_touchedPlatformSide == K.SIDE_DOWN)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.LEFT02, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.LEFT02);
                        }
                    }
                    if (currentGravity == Gravity.DOWN)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.DOWN02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.DOWN02);
                    }
                }
                if (_touchedPlatformSide == K.SIDE_RIGHT || _touchedPlatformSide == K.SIDE_LEFT)
                {
                    if (currentGravity == Gravity.UP)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.UP02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.UP02);
                    }
                    if (currentGravity == Gravity.LEFT)
                    {
                        if (_touchedPlatformSide == K.SIDE_RIGHT)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.LEFT01, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.LEFT01);
                        }
                        else if (_touchedPlatformSide == K.SIDE_LEFT)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.LEFT02, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.LEFT02);
                        }
                    }
                    if (currentGravity == Gravity.DOWN)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.DOWN01, _rotatingSpeed);
                        else
                            EndRotation(Rotation.DOWN01);
                    }
                }
            }
            if (previousGravity == Gravity.LEFT)
            {
                if (_touchedPlatformSide == K.SIDE_UP || _touchedPlatformSide == K.SIDE_DOWN)
                {
                    if (currentGravity == Gravity.UP)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.UP02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.UP02);
                    }
                    if (currentGravity == Gravity.RIGHT)
                    {
                        if (_touchedPlatformSide == K.SIDE_UP)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.RIGHT02, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.RIGHT02);
                        }
                        else if (_touchedPlatformSide == K.SIDE_DOWN)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.RIGHT01, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.RIGHT01);
                        }
                    }
                    if (currentGravity == Gravity.DOWN)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.DOWN02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.DOWN02);
                    }
                }
                if (_touchedPlatformSide == K.SIDE_RIGHT || _touchedPlatformSide == K.SIDE_LEFT)
                {
                    if (currentGravity == Gravity.UP)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.UP02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.UP02);
                    }
                    if (currentGravity == Gravity.RIGHT)
                    {
                        if (_touchedPlatformSide == K.SIDE_RIGHT)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.RIGHT02, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.RIGHT02);
                        }
                        else if (_touchedPlatformSide == K.SIDE_LEFT)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.RIGHT01, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.RIGHT01);
                        }
                    }
                    if (currentGravity == Gravity.DOWN)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.DOWN01, _rotatingSpeed);
                        else
                            EndRotation(Rotation.DOWN01);
                    }
                }
            }
            if (previousGravity == Gravity.DOWN)
            {
                if (_touchedPlatformSide == K.SIDE_UP || _touchedPlatformSide == K.SIDE_DOWN)
                {
                    if (currentGravity == Gravity.UP)
                    {
                        if (_touchedPlatformSide == K.SIDE_UP)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.UP02, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.UP02);
                        }
                        else if (_touchedPlatformSide == K.SIDE_DOWN)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.UP01, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.UP01);
                        }
                    }
                    if (currentGravity == Gravity.RIGHT)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.RIGHT02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.RIGHT02);
                    }
                    if (currentGravity == Gravity.LEFT)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.LEFT01, _rotatingSpeed);
                        else
                            EndRotation(Rotation.LEFT01);
                    }
                }
                if (_touchedPlatformSide == K.SIDE_RIGHT || _touchedPlatformSide == K.SIDE_LEFT)
                {
                    if (currentGravity == Gravity.UP)
                    {
                        if (_touchedPlatformSide == K.SIDE_RIGHT)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.UP01, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.UP01);
                        }
                        else if (_touchedPlatformSide == K.SIDE_LEFT)
                        {
                            if (_rotatingProgress <= 1f)
                                RotateThis(Rotation.UP02, _rotatingSpeed * 2f);
                            else
                                EndRotation(Rotation.UP02);
                        }
                    }
                    if (currentGravity == Gravity.RIGHT)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.RIGHT02, _rotatingSpeed);
                        else
                            EndRotation(Rotation.RIGHT02);
                    }
                    if (currentGravity == Gravity.LEFT)
                    {
                        if (_rotatingProgress <= 1f)
                            RotateThis(Rotation.LEFT01, _rotatingSpeed);
                        else
                            EndRotation(Rotation.LEFT01);
                    }
                }
            }
        }
    }

    void RotateThis(Quaternion finalRot, float speedReducer)
    {
        if (!_beforeRotationPositionSet)
        {
            _beforeRotationPosition = transform.position;
            _beforeRotationPositionSet = true;
        }

        transform.position = _beforeRotationPosition;
        velocity = Vector2.zero;

        if (!_originalRotationSet)
        {
            _originalRotation = transform.rotation;
            _originalRotationSet = true;
        }

        _rotatingProgress += Time.deltaTime / speedReducer;
        transform.rotation = Quaternion.Lerp(_originalRotation, finalRot, _rotatingProgress);
    }

    protected void EndRotation(Quaternion finalRot)
    {
        if (GetComponent<Player>())
        {
            var player = GetComponent<Player>();
            player.currentSpeed = player.walkSpeed;
            player.directionalInput = Vector2.zero;
        }
        transform.rotation = finalRot;
        rotating = false;
        locked = false;
        _originalRotation = transform.rotation;
        _originalRotationSet = false;
        _beforeRotationPositionSet = false;
        _rotatingProgress = 0f;
    }

    public bool CheckCollisionsAfterPush(Vector2 platformDirection, ref int rayCount, ref Vector2 raycastOrigin)
    {
        UpdateRayCastOrigins();
        var rayLenght = 1.5f;

        for (int i = 0; i < rayCount; i++)
        {
            var rayOrigin = raycastOrigin;
            if (rayCount == horizontalRayCount)
                rayOrigin += transformVector2.right * (horizontalRaySpacing * i + velocity.x);
            else if (rayCount == verticalRayCount)
                rayOrigin += transformVector2.up * (verticalRaySpacing * i);

            var hit = Physics2D.Raycast(rayOrigin, platformDirection, rayLenght, collisionMask);
            if (hit)
            {
                if (hit.collider.gameObject.layer == K.LAYER_OBSTACLE)
                {
                    if (hit.collider.gameObject.tag != K.TAG_PASSABLE)
                        return true;
                }
            }
        }

        return false;
    }

    public void EqualRotationToGravity()
    {
        if (transform.rotation.eulerAngles == Rotation.EULER_UP)
            SetBothGravities(Gravity.UP);
        else if (transform.rotation.eulerAngles == Rotation.EULER_RIGHT)
            SetBothGravities(Gravity.RIGHT);
        else if (transform.rotation.eulerAngles == Rotation.EULER_LEFT)
            SetBothGravities(Gravity.LEFT);
        else if (transform.rotation.eulerAngles == Rotation.EULER_DOWN)
            SetBothGravities(Gravity.DOWN);
    }

    void SetBothGravities(Vector2 gravity)
    {
        previousGravity = gravity;
        currentGravity = gravity;
    }

    protected void SaveGravityEntityData<T>(ref T gravityEntityData) where T : GravityEntityData
    {
        gravityEntityData.SetCurrentGravityValues(currentGravity.x, currentGravity.y);
    }

    protected void LoadGravityEntityData<T>(ref T gravityEntityData) where T : GravityEntityData
    {
        previousGravity = new Vector2((float)gravityEntityData.currentGravity[0], (float)gravityEntityData.currentGravity[1]);
        currentGravity = new Vector2((float)gravityEntityData.currentGravity[0], (float)gravityEntityData.currentGravity[1]);
        EqualRotationToGravity();
    }
}