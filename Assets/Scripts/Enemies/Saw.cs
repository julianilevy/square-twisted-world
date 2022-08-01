using UnityEngine;
using DarkTonic.MasterAudio;

public class Saw : EnemyMobileBase, ISavable, ILoadable<SawData>
{
    public Tuple<Ref<string>, int> rotationDirection;
    public Ref<float> rotationSpeed;

    private SawData _savedData;
    private SpriteRenderer[] _edges = new SpriteRenderer[2];
    private float _baseRotationSpeed = 5f;

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
        SetTileOverlap();
        _edges[0] = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _edges[1] = transform.GetChild(1).GetComponent<SpriteRenderer>();

        if (needsToBeCreated)
        {
            rotationDirection = Tuple.Create(Ref.Create(RotatingDirections.right), RotatingDirections.rightIndex);
            rotationSpeed = Ref.Create(75f);
        }
    }

    public override void Update()
    {
        base.Update();
        if (StartTimerIsOver())
            Rotate();
    }

    void Rotate()
    {
        if (rotationDirection.first.value == RotatingDirections.right)
        {
            for (int i = 0; i < _edges.Length; i++)
                _edges[i].transform.Rotate(Vector3.forward * _baseRotationSpeed * -rotationSpeed.value * Time.deltaTime);
        }
        else if (rotationDirection.first.value == RotatingDirections.left)
        {
            for (int i = 0; i < _edges.Length; i++)
                _edges[i].transform.Rotate(Vector3.forward * _baseRotationSpeed * rotationSpeed.value * Time.deltaTime);
        }
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new SawData();
            SaveTransformData(ref _savedData);
            SaveMobileBaseData(ref _savedData);
            _savedData.SetWaypointsData(savedWaypoints);
            _savedData.SetRotationDirectionValues(rotationDirection.first.value, rotationDirection.second, rotationSpeed.value);
        }
    }

    public void LoadData(SawData sawData)
    {
        _savedData = sawData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                LoadMobileBaseData(ref _savedData);
                rotationDirection = Tuple.Create(Ref.Create(_savedData.rotationDirectionName), _savedData.rotationDirectionIndex);
                rotationSpeed = Ref.Create((float)_savedData.rotationSpeed);
            }

            if (GameManager.instance.IsPlayingLevel())
                MasterAudio.PlaySound3DFollowTransform("Saw", transform);
        }
    }

    public TransformData GetSavedData()
    {
        if (_savedData != null)
            return _savedData;

        return null;
    }
}