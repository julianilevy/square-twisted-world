using DarkTonic.MasterAudio;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerSprite))]
public class Player : EntityMotor, ISavable, ILoadable<PlayerData>
{
    public LayerMask onlyMovingPlatformMask;
    public LayerMask onlyHandheldColliderMask;
    public Vector2 wallJumpClimb = new Vector2(65f, 55f);
    public Vector2 wallJumpOff = new Vector2(60f, 60f);
    public float walkSpeed = 22f;
    public float runSpeedWithObject = 35f;
    public float runSpeed = 35f;
    public float minJumpHeight = 3.5f;
    public float maxJumpHeight = 13f;
    public float timeToJumpStop = 0.45f;
    public float wallSlideSpeedMax = 15f;
    public float maxAccelerationTimeAir = 0.15f;
    public float maxAccelerationTimeGrounded = 0.05f;

    [HideInInspector]
    public PlayerInput input;
    [HideInInspector]
    public PlayerStats stats;
    [HideInInspector]
    public PlayerSprite sprite;
    [HideInInspector]
    public Camera originalPlayerCamera;
    [HideInInspector]
    public Camera playerCamera;
    [HideInInspector]
    public float currentSpeed;
    [HideInInspector]
    public bool movingPlatformSliding;
    [HideInInspector]
    public float platformVelocityX;
    [HideInInspector]
    public int wallDirX;
    [HideInInspector]
    public bool idle;
    [HideInInspector]
    public bool running;
    [HideInInspector]
    public bool wallSliding;
    [HideInInspector]
    public bool jumping;
    [HideInInspector]
    public bool jumpedWhileRunning;
    [HideInInspector]
    public bool falling;
    [HideInInspector]
    public bool fellWhileRunning;

    private PlayerData _savedData;
    private BoxCollider2D _collider2D;
    private Vector2 _previousGravity;
    private float _accelerationTimeAir;
    private float _accelerationTimeGrounded;
    private float _velocityXSmoothing;
    private float _minJumpVelocity;
    private float _maxJumpVelocity;
    private bool _fallingAfterWallJump;
    private bool _canDecreaseXVelocityAtObstacle;
    private bool _movingPlatformAhead;
    private int _fallingCounter;
    private int _slidingInMovingPlatformCounter;

    public override void Awake()
    {
        base.Awake();

        spawnAmountLimit = 1;
        input = GetComponent<PlayerInput>();
        stats = GetComponent<PlayerStats>();
        sprite = GetComponent<PlayerSprite>();
        gravityValue = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpStop, 2);
        _maxJumpVelocity = Mathf.Abs(gravityValue) * timeToJumpStop;
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravityValue) * minJumpHeight);
        _accelerationTimeAir = maxAccelerationTimeAir;
        _accelerationTimeGrounded = maxAccelerationTimeGrounded;
        _collider2D = GetComponent<BoxCollider2D>();
        currentSpeed = walkSpeed;
    }

    public override void Update()
    {
        if (!stats.dead)
        {
            base.Update();

            if (GameManager.instance.IsPlayingLevel())
            {
                if (!locked)
                {
                    CheckMovingPlatformAhead();
                    CalculateVelocity();
                    HandleWallSliding();
                    HandleMovement();
                    HandleVelocityAtFloor();
                    ResetMovingPlatformSliding();
                }
            }
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (!movingPlatformSliding)
        {
            if (!stats.isCarryingObject)
            {
                if (!falling || wallSliding)
                {
                    if (!jumping)
                        sprite.PlayAnimatorState("Jump");
                    jumping = true;
                    if (running)
                        jumpedWhileRunning = true;
                }

                if (wallSliding)
                {
                    _canDecreaseXVelocityAtObstacle = true;

                    SetDirectionalInput(new Vector2(-wallDirX, directionalInput.y));

                    if (wallDirX == directionalInput.x)
                    {
                        velocity.x = -wallDirX * wallJumpClimb.x;
                        velocity.y = wallJumpClimb.y;
                    }
                    else
                    {
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                        _fallingAfterWallJump = true;
                    }
                }
            }
            else
            {
                if (!jumping && !falling)
                {
                    if (Mathf.Abs(velocity.x) >= 5f)
                    {
                        if (velocity.x > 0f)
                        {
                            if (jumpedWhileRunning || running)
                                sprite.PlayAnimatorState("JumpRunningSideKey");
                            else
                                sprite.PlayAnimatorState("JumpSideKey");
                        }
                        else if (velocity.x < 0f)
                        {
                            if (jumpedWhileRunning || running)
                                sprite.PlayAnimatorState("JumpRunningSideKeyLeft");
                            else
                                sprite.PlayAnimatorState("JumpSideKeyLeft");
                        }
                    }
                    else
                    {
                        if (!jumpedWhileRunning)
                            sprite.PlayAnimatorState("JumpFrontKey");
                    }
                }
                jumping = true;
                if (running)
                    jumpedWhileRunning = true;
            }
            if (collisions.down)
            {
                _canDecreaseXVelocityAtObstacle = true;
                velocity.y = _maxJumpVelocity;
            }
        }
        else
        {
            if (!stats.isCarryingObject)
            {
                if (!jumping)
                {
                    _slidingInMovingPlatformCounter = 0;
                    sprite.PlayAnimatorState("Jump");
                    SetDirectionalInput(new Vector2(-wallDirX, directionalInput.y));
                }
                jumping = true;
                if (running)
                    jumpedWhileRunning = true;

                if (wallDirX == directionalInput.x)
                {
                    velocity.x = -wallDirX * (wallJumpClimb.x + (platformVelocityX / Time.deltaTime));
                    velocity.y = wallJumpClimb.y;
                    alreadyFallingFromMovingPlatformSlicing = false;
                }
                else
                {
                    velocity.x = -wallDirX * (wallJumpOff.x + (platformVelocityX / Time.deltaTime)); // Testear...
                    //velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                    _fallingAfterWallJump = true;
                    alreadyFallingFromMovingPlatformSlicing = false;
                }
            }
            else
            {
                if (!jumping && !falling)
                {
                    _slidingInMovingPlatformCounter = 0;
                    if (Mathf.Abs(velocity.x) >= 5f)
                    {
                        if (velocity.x > 0f)
                        {
                            if (jumpedWhileRunning || running)
                                sprite.PlayAnimatorState("JumpRunningSideKey");
                            else
                                sprite.PlayAnimatorState("JumpSideKey");
                        }
                        else if (velocity.x < 0f)
                        {
                            if (jumpedWhileRunning || running)
                                sprite.PlayAnimatorState("JumpRunningSideKeyLeft");
                            else
                                sprite.PlayAnimatorState("JumpSideKeyLeft");
                        }
                    }
                    else
                    {
                        if (!jumpedWhileRunning)
                            sprite.PlayAnimatorState("JumpFrontKey");
                    }
                }
                jumping = true;
                if (running)
                    jumpedWhileRunning = true;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > _minJumpVelocity)
            velocity.y = _minJumpVelocity;
    }

    public void SetRunAxis(float axisValue)
    {
        if (axisValue == 1)
        {
            if (!stats.isCarryingObject)
                currentSpeed = runSpeed;
            else
                currentSpeed = runSpeedWithObject;
            running = true;
        }
        else
        {
            currentSpeed = walkSpeed;
            running = false;
        }
    }

    public void OnRunInput()
    {
        if (!stats.isCarryingObject)
            currentSpeed = runSpeed;
        else
            currentSpeed = runSpeedWithObject;
        running = true;
    }

    public void OnRunInputUp()
    {
        currentSpeed = walkSpeed;
        running = false;
    }

    public void OnInteractWithObject()
    {
        if (!stats.isCarryingObject)
            PickObject();
        else
            DropObject();
    }

    void CheckMovingPlatformAhead()
    {
        if (!wallSliding)
        {
            if (!collisions.down)
            {
                var directionX = Mathf.Sign(velocity.x);
                var rayCount = 3;
                var rayLenght = 3f;
                var raySeparation = 1.38f;

                for (int i = 0; i < rayCount; i++)
                {
                    Vector2 rayOrigin = Vector2.zero;
                    if (directionX == 1)
                        rayOrigin = raycastOrigins.topRight;
                    else
                        rayOrigin = raycastOrigins.topLeft;
                    rayOrigin += -transformVector2.up * (i * raySeparation);

                    // Puede mejorarse la precisión viendo si esta Horizontal o Vertical, y dependiendo de la gravedad actual.

                    var hit = Physics2D.Raycast(rayOrigin, transform.right * directionX, rayLenght, onlyMovingPlatformMask);
                    if (hit)
                    {
                        _movingPlatformAhead = true;
                        return;
                    }
                    else
                        _movingPlatformAhead = false;
                }
            }
            else
                _movingPlatformAhead = false;
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * currentSpeed;

        if (collisions.down)
        {
            platformVelocityX = 0;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _velocityXSmoothing, _accelerationTimeGrounded);
        }
        else
        {
            if (directionalInput.x == 0)
                _velocityXSmoothing = 0;

            if (!alreadyFallingFromMovingPlatformSlicing)
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref _velocityXSmoothing, _accelerationTimeAir);
            else
            {
                _velocityXSmoothing = 0;
                velocity.x = 0;
            }
        }

        if (velocity.y <= 0)
        {
            jumping = false;
            jumpedWhileRunning = false;
        }

        velocity.y += gravityValue * Time.deltaTime;
    }

    void HandleWallSliding()
    {
        if (!stats.isCarryingObject)
        {
            if (collisions.left)
                wallDirX = -1;
            else
                wallDirX = 1;

            wallSliding = false;

            if ((collisions.left || collisions.right) && !collisions.down && velocity.y < _maxJumpVelocity / 1.5f && !movingPlatformSliding)
            {
                if (!_movingPlatformAhead)
                {
                    if (_fallingAfterWallJump)
                    {
                        velocity.y /= 1.5f;
                        _fallingAfterWallJump = false;
                    }

                    if (_canDecreaseXVelocityAtObstacle)
                    {
                        velocity.x = 0;
                        _velocityXSmoothing = 0;
                        _canDecreaseXVelocityAtObstacle = false;
                    }

                    if (allVerticalRaysTouchingObstacle)
                    {
                        if (directionalInput.x == 0 || directionalInput.x == wallDirX)
                        {
                            velocity.x = 0;
                            _velocityXSmoothing = 0;
                        }
                    }

                    if (collisions.left)
                        SetDirectionalInput(new Vector2(-1, directionalInput.y));
                    else if (collisions.right)
                        SetDirectionalInput(new Vector2(1, directionalInput.y));

                    jumping = false;
                    jumpedWhileRunning = false;
                    wallSliding = true;
                    sprite.PlayAnimatorState("WallSlide");

                    if (velocity.y < -wallSlideSpeedMax)
                        velocity.y = -wallSlideSpeedMax;
                }
            }
            if (movingPlatformSliding)
            {
                _slidingInMovingPlatformCounter++;
                if (_slidingInMovingPlatformCounter >= 5)
                {
                    if (collisions.left)
                        SetDirectionalInput(new Vector2(-1, directionalInput.y));
                    else if (collisions.right)
                        SetDirectionalInput(new Vector2(1, directionalInput.y));

                    jumping = false;
                    jumpedWhileRunning = false;
                    wallSliding = true;
                    sprite.PlayAnimatorState("WallSlide");
                }

                if (velocity.y < -wallSlideSpeedMax)
                    velocity.y = -wallSlideSpeedMax;
            }
            else
                _slidingInMovingPlatformCounter = 0;
        }
        else
        {
            if (wallSliding)
                wallSliding = false;
        }
    }

    void HandleMovement()
    {
        Move(velocity * Time.deltaTime);
    }

    void HandleVelocityAtFloor()
    {
        if (collisions.up || collisions.down)
        {
            velocity.y = 0;

            _fallingCounter++;
            if (_fallingCounter >= 2)
            {
                falling = false;
                fellWhileRunning = false;
            }

            if (collisions.down && _fallingAfterWallJump)
                _fallingAfterWallJump = false;
        }
        else
        {
            _fallingCounter = 0;
            if (jumpedWhileRunning)
                fellWhileRunning = true;
            else if (running && !falling)
                fellWhileRunning = true;
            falling = true;
        }
    }

    void ResetMovingPlatformSliding()
    {
        movingPlatformSliding = false;
    }

    public void GetDamage(int damage)
    {
        stats.GetDamage(damage);
    }

    public void PickObject()
    {
        MasterAudio.PlaySound3DFollowTransformAndForget("GrabKey", transform);
        stats.PickObject();
    }

    public void DropObject()
    {
        stats.DropObject();
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new PlayerData();
            SaveTransformData(ref _savedData);
            SaveGravityEntityData(ref _savedData);
        }
    }

    public void LoadData(PlayerData playerData)
    {
        _savedData = playerData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                LoadGravityEntityData(ref _savedData);

                if (GameManager.instance.IsPlayingLevel())
                {
                    playerCamera.gameObject.SetActive(true);
                    playerCamera.GetComponent<AudioListener>().enabled = true;
                    playerCamera.enabled = true;
                }
            }
        }
    }

    public TransformData GetSavedData()
    {
        if (_savedData != null)
            return _savedData;

        return null;
    }

    protected override void OnEntityPushed()
    {
        if (!stats.isCarryingObject)
            sprite.PlayAnimatorState("Idle");
        else
            sprite.PlayAnimatorState("IdleKey");
    }
}