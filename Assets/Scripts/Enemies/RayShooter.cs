using DarkTonic.MasterAudio;
using UnityEngine;

public class RayShooter : EnemyMobileBase, ISavable, ILoadable<RayShooterData>
{
    public Ref<bool> rayUp;
    public Ref<float> rayUpIntervalIntermittenceTime;
    public Ref<float> rayUpIntervalDurationTime;
    public Ref<bool> rayRight;
    public Ref<float> rayRightIntervalIntermittenceTime;
    public Ref<float> rayRightIntervalDurationTime;
    public Ref<bool> rayLeft;
    public Ref<float> rayLeftIntervalIntermittenceTime;
    public Ref<float> rayLeftIntervalDurationTime;
    public Ref<bool> rayDown;
    public Ref<float> rayDownIntervalIntermittenceTime;
    public Ref<float> rayDownIntervalDurationTime;
    public Tuple<Ref<string>, int> rotationDirection;
    public Ref<float> rotationSpeed;

    [HideInInspector]
    public GameObject baseCenter;
    [HideInInspector]
    public LineRenderer shooterUp;
    [HideInInspector]
    public LineRenderer shooterRight;
    [HideInInspector]
    public LineRenderer shooterLeft;
    [HideInInspector]
    public LineRenderer shooterDown;
    [HideInInspector]
    public GameObject baseUpEmission;
    [HideInInspector]
    public GameObject baseRightEmission;
    [HideInInspector]
    public GameObject baseLeftEmission;
    [HideInInspector]
    public GameObject baseDownEmission;
    [HideInInspector]
    public Transform rayUpSpawnPoint;
    [HideInInspector]
    public Transform rayRightSpawnPoint;
    [HideInInspector]
    public Transform rayLeftSpawnPoint;
    [HideInInspector]
    public Transform rayDownSpawnPoint;
    [HideInInspector]
    public GameObject rayUpElectricity;
    [HideInInspector]
    public GameObject rayRightElectricity;
    [HideInInspector]
    public GameObject rayLeftElectricity;
    [HideInInspector]
    public GameObject rayDownElectricity;
    [HideInInspector]
    public ParticleSystem rayUpHittingParticles;
    [HideInInspector]
    public ParticleSystem rayRightHittingParticles;
    [HideInInspector]
    public ParticleSystem rayLeftHittingParticles;
    [HideInInspector]
    public ParticleSystem rayDownHittingParticles;

    private RayShooterData _savedData;
    private float _rayUpIntermittenceTimer;
    private float _rayUpDurationTimer;
    private float _rayRightIntermittenceTimer;
    private float _rayRightDurationTimer;
    private float _rayLeftIntermittenceTimer;
    private float _rayLeftDurationTimer;
    private float _rayDownIntermittenceTimer;
    private float _rayDownDurationTimer;
    private float _baseRotationSpeed = 5f;
    private Ref<bool> rayUpSoundsEnabled;
    private Ref<bool> rayRightSoundsEnabled;
    private Ref<bool> rayLeftSoundsEnabled;
    private Ref<bool> rayDownSoundsEnabled;

    public struct RotatingDirections
    {
        public static string right = "Right";
        public static int rightIndex = 0;
        public static string left = "Left";
        public static int leftIndex = 1;
    }

    public override void Awake()
    {
        base.Awake();

        if (needsToBeCreated)
        {
            rayUp = Ref.Create(true);
            rayUpIntervalIntermittenceTime = Ref.Create(0f);
            rayUpIntervalDurationTime = Ref.Create(0f);
            rayRight = Ref.Create(true);
            rayRightIntervalIntermittenceTime = Ref.Create(0f);
            rayRightIntervalDurationTime = Ref.Create(0f);
            rayLeft = Ref.Create(true);
            rayLeftIntervalIntermittenceTime = Ref.Create(0f);
            rayLeftIntervalDurationTime = Ref.Create(0f);
            rayDown = Ref.Create(true);
            rayDownIntervalIntermittenceTime = Ref.Create(0f);
            rayDownIntervalDurationTime = Ref.Create(0f);
            rotationDirection = Tuple.Create(Ref.Create(RotatingDirections.right), RotatingDirections.rightIndex);
            rotationSpeed = Ref.Create(0f);
            AwakeValues();
        }
    }

    void AwakeValues()
    {
        rayUpSoundsEnabled = Ref.Create(true);
        rayRightSoundsEnabled = Ref.Create(true);
        rayLeftSoundsEnabled = Ref.Create(true);
        rayDownSoundsEnabled = Ref.Create(true);
        _rayUpIntermittenceTimer = rayUpIntervalIntermittenceTime.value;
        _rayUpDurationTimer = rayUpIntervalDurationTime.value;
        _rayRightIntermittenceTimer = rayRightIntervalIntermittenceTime.value;
        _rayRightDurationTimer = rayRightIntervalDurationTime.value;
        _rayLeftIntermittenceTimer = rayLeftIntervalIntermittenceTime.value;
        _rayLeftDurationTimer = rayLeftIntervalDurationTime.value;
        _rayDownIntermittenceTimer = rayDownIntervalIntermittenceTime.value;
        _rayDownDurationTimer = rayDownIntervalDurationTime.value;
    }

    public override void Update()
    {
        base.Update();
        if (StartTimerIsOver())
        {
            IntermittenceTimers();
            Rotate();
        }
    }

    void LateUpdate()
    {
        if (StartTimerIsOver())
            DrawRay();
    }

    void DrawRay()
    {
        RaycastHit2D hitUp = new RaycastHit2D();
        RaycastHit2D hitRight = new RaycastHit2D();
        RaycastHit2D hitLeft = new RaycastHit2D();
        RaycastHit2D hitDown = new RaycastHit2D();

        if (rayUp.value)
        {
            EnableShooter(shooterUp.gameObject, baseUpEmission, true);
            CheckRayHit(hitUp, rayUpSpawnPoint, rayUpHittingParticles.gameObject, rayUpSoundsEnabled, baseCenter.transform.up, _rayUpDurationTimer, rayUpIntervalDurationTime.value, rayUpElectricity, shooterUp);
        }
        else
            EnableShooter(shooterUp.gameObject, baseUpEmission, false);
        if (rayRight.value)
        {
            EnableShooter(shooterRight.gameObject, baseRightEmission, true);
            CheckRayHit(hitRight, rayRightSpawnPoint, rayRightHittingParticles.gameObject, rayRightSoundsEnabled, baseCenter.transform.right, _rayRightDurationTimer, rayRightIntervalDurationTime.value, rayRightElectricity, shooterRight);
        }
        else
            EnableShooter(shooterRight.gameObject, baseRightEmission, false);
        if (rayLeft.value)
        {
            EnableShooter(shooterLeft.gameObject, baseLeftEmission, true);
            CheckRayHit(hitLeft, rayLeftSpawnPoint, rayLeftHittingParticles.gameObject, rayLeftSoundsEnabled, - baseCenter.transform.right, _rayLeftDurationTimer, rayLeftIntervalDurationTime.value, rayLeftElectricity, shooterLeft);
        }
        else
            EnableShooter(shooterLeft.gameObject, baseLeftEmission, false);
        if (rayDown.value)
        {
            EnableShooter(shooterDown.gameObject, baseDownEmission, true);
            CheckRayHit(hitDown, rayDownSpawnPoint, rayDownHittingParticles.gameObject, rayDownSoundsEnabled, - baseCenter.transform.up, _rayDownDurationTimer, rayDownIntervalDurationTime.value, rayDownElectricity, shooterDown);
        }
        else
            EnableShooter(shooterDown.gameObject, baseDownEmission, false);
    }

    void EnableShooter(GameObject shooter, GameObject baseEmission, bool value)
    {
        shooter.SetActive(value);
        baseEmission.SetActive(value);
    }

    void CheckRayHit(RaycastHit2D hit, Transform spawnPosition, GameObject hittingParticles, Ref<bool> raySoundsEnabled, Vector3 direction, float durationTimer, float intervalDurationTime, GameObject electricity, LineRenderer lineRenderer)
    {
        hit = Physics2D.Raycast(spawnPosition.position, direction, Mathf.Infinity, collisionMask);

        if (hit)
        {
            if (durationTimer == intervalDurationTime)
            {
                electricity.SetActive(true);
                hittingParticles.SetActive(true);
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, spawnPosition.position);
                lineRenderer.SetPosition(1, hit.point);
                hittingParticles.transform.position = hit.point;
                if (!raySoundsEnabled.value)
                {
                    raySoundsEnabled.value = true;
                    MasterAudio.PlaySound3DFollowTransform("Laser", hittingParticles.transform);
                }

                if (!GameManager.instance.IsUsingLevelMaker())
                {
                    if (hit.transform.gameObject.layer == K.LAYER_PLAYER)
                    {
                        var player = hit.transform.GetComponent<Player>();
                        player.GetDamage(damage);
                    }
                }
            }
            else
            {
                electricity.SetActive(false);
                hittingParticles.SetActive(false);
                lineRenderer.positionCount = 0;
                if (raySoundsEnabled.value)
                {
                    raySoundsEnabled.value = false;
                    MasterAudio.FadeOutAllSoundsOfTransform(hittingParticles.transform, 0.1f);
                }
            }
        }
    }

    void IntermittenceTimers()
    {
        CheckIntermittenceTimer(rayUpIntervalIntermittenceTime.value, ref _rayUpIntermittenceTimer, rayUpIntervalDurationTime.value, ref _rayUpDurationTimer);
        CheckIntermittenceTimer(rayRightIntervalIntermittenceTime.value, ref _rayRightIntermittenceTimer, rayRightIntervalDurationTime.value, ref _rayRightDurationTimer);
        CheckIntermittenceTimer(rayLeftIntervalIntermittenceTime.value, ref _rayLeftIntermittenceTimer, rayLeftIntervalDurationTime.value, ref _rayLeftDurationTimer);
        CheckIntermittenceTimer(rayDownIntervalIntermittenceTime.value, ref _rayDownIntermittenceTimer, rayDownIntervalDurationTime.value, ref _rayDownDurationTimer);
    }

    void CheckIntermittenceTimer(float intermittenceTime, ref float intermittenceTimer, float durationTime, ref float durationTimer)
    {
        if (intermittenceTime > 0)
        {
            intermittenceTimer -= Time.deltaTime;
            if (intermittenceTimer <= 0)
            {
                durationTimer -= Time.deltaTime;
                if (durationTimer <= 0)
                {
                    intermittenceTimer = intermittenceTime;
                    durationTimer = durationTime;
                }
            }
        }
    }

    void Rotate()
    {
        var finalRotationSpeed = Mathf.Clamp(_baseRotationSpeed * rotationSpeed.value, 0f, 4000f);
        if (rotationDirection.first.value == RotatingDirections.right)
        {
            baseCenter.transform.Rotate(Vector3.forward * -finalRotationSpeed * Time.deltaTime);
            rotationDirection.first.value = RotatingDirections.right;
        }
        else if (rotationDirection.first.value == RotatingDirections.left)
        {
            baseCenter.transform.Rotate(Vector3.forward * finalRotationSpeed * Time.deltaTime);
            rotationDirection.first.value = RotatingDirections.left;
        }
    }

    public override BasePrefab EqualBoolToBasePrefab(Ref<bool> boolToEqual)
    {
        if (boolToEqual == rayUp)
            return shooterUp.GetComponent<BasePrefab>();
        else if (boolToEqual == rayRight)
            return shooterRight.GetComponent<BasePrefab>();
        else if (boolToEqual == rayLeft)
            return shooterLeft.GetComponent<BasePrefab>();
        else if (boolToEqual == rayDown)
            return shooterDown.GetComponent<BasePrefab>();

        return null;
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new RayShooterData();
            SaveTransformData(ref _savedData);
            SaveMobileBaseData(ref _savedData);
            _savedData.SetWaypointsData(savedWaypoints);
            _savedData.SetRayUpValues(rayUp.value, rayUpIntervalIntermittenceTime.value, rayUpIntervalDurationTime.value);
            _savedData.SetRayRightValues(rayRight.value, rayRightIntervalIntermittenceTime.value, rayRightIntervalDurationTime.value);
            _savedData.SetRayLeftValues(rayLeft.value, rayLeftIntervalIntermittenceTime.value, rayLeftIntervalDurationTime.value);
            _savedData.SetRayDownValues(rayDown.value, rayDownIntervalIntermittenceTime.value, rayDownIntervalDurationTime.value);
            _savedData.SetRotationDirectionValues(rotationDirection.first.value, rotationDirection.second, rotationSpeed.value);
        }
    }

    public void LoadData(RayShooterData rayShooterData)
    {
        _savedData = rayShooterData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                LoadMobileBaseData(ref _savedData);
                rayUp = Ref.Create(rayShooterData.rayUp);
                rayUpIntervalIntermittenceTime = Ref.Create((float)_savedData.rayUpIntervalIntermittenceTime);
                rayUpIntervalDurationTime = Ref.Create((float)_savedData.rayUpIntervalDurationTime);
                rayRight = Ref.Create(_savedData.rayRight);
                rayRightIntervalIntermittenceTime = Ref.Create((float)_savedData.rayRightIntervalIntermittenceTime);
                rayRightIntervalDurationTime = Ref.Create((float)_savedData.rayRightIntervalDurationTime);
                rayLeft = Ref.Create(_savedData.rayLeft);
                rayLeftIntervalIntermittenceTime = Ref.Create((float)_savedData.rayLeftIntervalIntermittenceTime);
                rayLeftIntervalDurationTime = Ref.Create((float)_savedData.rayLeftIntervalDurationTime);
                rayDown = Ref.Create(_savedData.rayDown);
                rayDownIntervalIntermittenceTime = Ref.Create((float)_savedData.rayDownIntervalIntermittenceTime);
                rayDownIntervalDurationTime = Ref.Create((float)_savedData.rayDownIntervalDurationTime);
                rotationDirection = Tuple.Create(Ref.Create(_savedData.rotationDirectionName), _savedData.rotationDirectionIndex);
                rotationSpeed = Ref.Create((float)_savedData.rotationSpeed);
                AwakeValues();
            }

            if (GameManager.instance.IsPlayingLevel())
            {
                if (rayUp.value)
                {
                    MasterAudio.PlaySound3DFollowTransform("Laser", rayUpHittingParticles.transform);
                    rayUpSoundsEnabled.value = true;
                }
                if (rayRight.value)
                {
                    MasterAudio.PlaySound3DFollowTransform("Laser", rayRightHittingParticles.transform);
                    rayRightSoundsEnabled.value = true;
                }
                if (rayLeft.value)
                {
                    MasterAudio.PlaySound3DFollowTransform("Laser", rayLeftHittingParticles.transform);
                    rayLeftSoundsEnabled.value = true;
                }
                if (rayDown.value)
                {
                    MasterAudio.PlaySound3DFollowTransform("Laser", rayDownHittingParticles.transform);
                    rayDownSoundsEnabled.value = true;
                }

                MasterAudio.PlaySound3DFollowTransform("RayShooter", transform);
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