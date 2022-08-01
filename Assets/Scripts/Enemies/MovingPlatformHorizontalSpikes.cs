using UnityEngine;

public class MovingPlatformHorizontalSpikes : HarmlessMobileBase, ISavable, ILoadable<MovingPlatformHorizontalSpikesData>
{
    [HideInInspector]
    public SpriteRenderer platformSprite;
    [HideInInspector]
    public string platformType;

    private MovingPlatformHorizontalSpikesData _savedData;
    private float _colliderOriginalSizeX = 6.4f;
    private float _colliderOriginalSizeY = 1.9f;

    public override void Awake()
    {
        base.Awake();
        canBeRotated = true;

        if (needsToBeCreated)
            SetType(PlatformType.horizontal);
    }

    public void Rotate()
    {
        if (platformSprite.transform.rotation.eulerAngles == Rotation.EULER_DOWN)
        {
            platformSprite.transform.rotation = Quaternion.Euler(Rotation.EULER_LEFT);
            GetComponent<BoxCollider2D>().size = new Vector2(_colliderOriginalSizeY, _colliderOriginalSizeX);
            platformType = PlatformType.vertical;
        }
        else if (platformSprite.transform.rotation.eulerAngles == Rotation.EULER_LEFT)
        {
            platformSprite.transform.rotation = Quaternion.Euler(Rotation.EULER_DOWN);
            GetComponent<BoxCollider2D>().size = new Vector2(_colliderOriginalSizeX, _colliderOriginalSizeY);
            platformType = PlatformType.horizontal;
        }
    }

    public void SetType(string type)
    {
        platformType = type;
        EqualRotationToType();
        CalculateRaySpacing();
    }

    void EqualRotationToType()
    {
        if (platformType == PlatformType.horizontal)
        {
            platformSprite.transform.rotation = Quaternion.Euler(Rotation.EULER_DOWN);
            GetComponent<BoxCollider2D>().size = new Vector2(_colliderOriginalSizeX, _colliderOriginalSizeY);
        }
        else if (platformType == PlatformType.vertical)
        {
            platformSprite.transform.rotation = Quaternion.Euler(Rotation.EULER_LEFT);
            GetComponent<BoxCollider2D>().size = new Vector2(_colliderOriginalSizeY, _colliderOriginalSizeX);
        }
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new MovingPlatformHorizontalSpikesData();
            SaveTransformData(ref _savedData);
            SaveMobileBaseData(ref _savedData);
            _savedData.SetWaypointsData(savedWaypoints);
            _savedData.SetPlatformType(platformType);
        }
    }

    public void LoadData(MovingPlatformHorizontalSpikesData movingPlatformData)
    {
        _savedData = movingPlatformData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                LoadMobileBaseData(ref _savedData);
                SetType(_savedData.platformType);
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