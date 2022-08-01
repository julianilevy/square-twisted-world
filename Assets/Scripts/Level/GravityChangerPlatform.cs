using UnityEngine;

public class GravityChangerPlatform : RaycastController, ISavable, ILoadable<GravityChangerPlatformData>
{
    public Tuple<Ref<string>, int> nextGravity;

    [HideInInspector]
    public SpriteRenderer emission;
    [HideInInspector]
    public ParticleSystem particlesUp;
    [HideInInspector]
    public ParticleSystem particlesRight;
    [HideInInspector]
    public ParticleSystem particlesLeft;
    [HideInInspector]
    public ParticleSystem particlesDown;
    [HideInInspector]
    public SpriteRenderer arrowUp;
    [HideInInspector]
    public SpriteRenderer arrowRight;
    [HideInInspector]
    public SpriteRenderer arrowLeft;
    [HideInInspector]
    public SpriteRenderer arrowDown;

    private GravityChangerPlatformData _savedData;
    private Vector2 _nextGravity;
    private Vector2 _gravityForChecking;
    private Quaternion _arrowRotation;
    private string _currentSide;
    private string _sideForCheking;
    private float _jumpForce = 45f;

    public override void Awake()
    {
        base.Awake();

        if (needsToBeCreated)
        {
            canBeRotated = true;
            snapToTile = true;
            snapValueY = 1.1f;
            nextGravity = Tuple.Create(Ref.Create(Gravity.Forces.down), Gravity.Forces.downIndex);
            AwakeValues();
        }
    }

    void AwakeValues()
    {
        _nextGravity = Gravity.GetGravityByForces(nextGravity.first.value);
        _arrowRotation = Quaternion.identity;
        EqualNextGravities();
        CheckCurrentSide();
        SelectColor();
        SelectParticles();
        KeepArrowRotation();
    }

    public override void Update()
    {
        base.Update();

        if (GameManager.instance.IsUsingLevelMaker())
            AwakeValues();
        else
            CheckPlatformHit();
    }

    void EqualNextGravities()
    {
        if (_nextGravity == Gravity.UP)
        {
            nextGravity.first.value = Gravity.Forces.up;
            nextGravity.second = Gravity.Forces.upIndex;
        }
        else if (_nextGravity == Gravity.RIGHT)
        {
            nextGravity.first.value = Gravity.Forces.right;
            nextGravity.second = Gravity.Forces.rightIndex;
        }
        else if (_nextGravity == Gravity.LEFT)
        {
            nextGravity.first.value = Gravity.Forces.left;
            nextGravity.second = Gravity.Forces.leftIndex;
        }
        else if (_nextGravity == Gravity.DOWN)
        {
            nextGravity.first.value = Gravity.Forces.down;
            nextGravity.second = Gravity.Forces.downIndex;
        }
    }

    void CheckCurrentSide()
    {
        if (transform.rotation.eulerAngles == Rotation.EULER_UP)
            _currentSide = K.SIDE_UP;
        else if (transform.rotation.eulerAngles == Rotation.EULER_RIGHT)
            _currentSide = K.SIDE_RIGHT;
        else if (transform.rotation.eulerAngles == Rotation.EULER_LEFT)
            _currentSide = K.SIDE_LEFT;
        else if (transform.rotation.eulerAngles == Rotation.EULER_DOWN)
            _currentSide = K.SIDE_DOWN;
    }

    void SelectColor()
    {
        if (_gravityForChecking != _nextGravity)
        {
            arrowUp.gameObject.SetActive(false);
            arrowRight.gameObject.SetActive(false);
            arrowLeft.gameObject.SetActive(false);
            arrowDown.gameObject.SetActive(false);

            if (_nextGravity == Gravity.UP)
                SelectColor(Gravity.COLOR_UP, arrowUp);
            else if (_nextGravity == Gravity.RIGHT)
                SelectColor(Gravity.COLOR_RIGHT, arrowRight);
            else if (_nextGravity == Gravity.LEFT)
                SelectColor(Gravity.COLOR_LEFT, arrowLeft);
            else if (_nextGravity == Gravity.DOWN)
                SelectColor(Gravity.COLOR_DOWN, arrowDown);
        }
    }

    void SelectColor(Color gravityColor, SpriteRenderer arrow)
    {
        emission.material.color = gravityColor;
        arrow.gameObject.SetActive(true);
        arrow.material.color = gravityColor;
    }

    void SelectParticles()
    {
        if (_gravityForChecking != _nextGravity || _sideForCheking != _currentSide)
        {
            particlesUp.gameObject.SetActive(false);
            particlesRight.gameObject.SetActive(false);
            particlesLeft.gameObject.SetActive(false);
            particlesDown.gameObject.SetActive(false);

            if (_nextGravity == Gravity.UP)
            {
                if (_currentSide == K.SIDE_UP)
                    SelectParticles(Gravity.COLOR_UP, particlesDown);
                else if (_currentSide == K.SIDE_RIGHT)
                    SelectParticles(Gravity.COLOR_UP, particlesRight);
                else if (_currentSide == K.SIDE_LEFT)
                    SelectParticles(Gravity.COLOR_UP, particlesLeft);
                else if (_currentSide == K.SIDE_DOWN)
                    SelectParticles(Gravity.COLOR_UP, particlesUp);
            }
            else if (_nextGravity == Gravity.RIGHT)
            {
                if (_currentSide == K.SIDE_UP)
                    SelectParticles(Gravity.COLOR_RIGHT, particlesLeft);
                else if (_currentSide == K.SIDE_RIGHT)
                    SelectParticles(Gravity.COLOR_RIGHT, particlesDown);
                else if (_currentSide == K.SIDE_LEFT)
                    SelectParticles(Gravity.COLOR_RIGHT, particlesUp);
                else if (_currentSide == K.SIDE_DOWN)
                    SelectParticles(Gravity.COLOR_RIGHT, particlesRight);
            }
            else if (_nextGravity == Gravity.LEFT)
            {
                if (_currentSide == K.SIDE_UP)
                    SelectParticles(Gravity.COLOR_LEFT, particlesRight);
                else if (_currentSide == K.SIDE_RIGHT)
                    SelectParticles(Gravity.COLOR_LEFT, particlesUp);
                else if (_currentSide == K.SIDE_LEFT)
                    SelectParticles(Gravity.COLOR_LEFT, particlesDown);
                else if (_currentSide == K.SIDE_DOWN)
                    SelectParticles(Gravity.COLOR_LEFT, particlesLeft);
            }
            else if (_nextGravity == Gravity.DOWN)
            {
                if (_currentSide == K.SIDE_UP)
                    SelectParticles(Gravity.COLOR_DOWN, particlesUp);
                else if (_currentSide == K.SIDE_RIGHT)
                    SelectParticles(Gravity.COLOR_DOWN, particlesLeft);
                else if (_currentSide == K.SIDE_LEFT)
                    SelectParticles(Gravity.COLOR_DOWN, particlesRight);
                else if (_currentSide == K.SIDE_DOWN)
                    SelectParticles(Gravity.COLOR_DOWN, particlesDown);
            }
        }

        _gravityForChecking = _nextGravity;
        _sideForCheking = _currentSide;
    }

    void SelectParticles(Color gravityColor, ParticleSystem particles)
    {
        particles.gameObject.SetActive(true);
        var main = particles.main;
        main.startColor = gravityColor;
        _gravityForChecking = _nextGravity;
        _sideForCheking = _currentSide;
    }

    void KeepArrowRotation()
    {
        if (arrowUp.gameObject.activeSelf)
            arrowUp.transform.rotation = _arrowRotation;
        else if (arrowRight.gameObject.activeSelf)
            arrowRight.transform.rotation = _arrowRotation;
        else if (arrowLeft.gameObject.activeSelf)
            arrowLeft.transform.rotation = _arrowRotation;
        else if (arrowDown.gameObject.activeSelf)
            arrowDown.transform.rotation = _arrowRotation;
    }

    void CheckPlatformHit()
    {
        UpdateRayCastOrigins();
        var rayLenght = 0.2f;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            var rayOrigin = raycastOrigins.topLeft;
            rayOrigin += transformVector2.right * (horizontalRaySpacing * i);
            var hits = Physics2D.RaycastAll(rayOrigin, transform.up, rayLenght, collisionMask);

            foreach (var hit in hits)
            {
                if (hit)
                {
                    if (hit.transform.gameObject.layer == K.LAYER_PLAYER || hit.transform.gameObject.layer == K.LAYER_GRAVITYENTITY)
                    {
                        var tempGravityEntity = hit.transform.GetComponent<GravityEntity>();

                        if (!tempGravityEntity.pushedByGravityPlatform && !tempGravityEntity.rotating)
                        {
                            if (_nextGravity == tempGravityEntity.currentGravity || tempGravityEntity.pushedByGravityPlatform)
                                continue;

                            tempGravityEntity.SetNewGravity(_nextGravity, _currentSide);
                            tempGravityEntity.PushEntity(_jumpForce, transform.up);
                        }
                    }
                }
            }
        }
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new GravityChangerPlatformData();
            SaveTransformData(ref _savedData);
            _savedData.SetNextGravityValues(nextGravity.first.value, nextGravity.second);
        }
    }

    public void LoadData(GravityChangerPlatformData gravityChangerPlatformData)
    {
        _savedData = gravityChangerPlatformData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                nextGravity = Tuple.Create(Ref.Create(_savedData.nextGravityName), _savedData.nextGravityIndex);
                AwakeValues();
            }
        }
    }

    public TransformData GetSavedData()
    {
        if (_savedData != null)
            return _savedData;

        return null;
    }
}