public class Spike : EnemyBase, ISavable, ILoadable<SpikeData>
{
    private SpikeData _savedData;

    public override void Awake()
    {
        base.Awake();
        canContinuousPlacing = true;
        canBeRotated = true;
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new SpikeData();
            SaveTransformData(ref _savedData);
        }
    }

    public void LoadData(SpikeData spikeData)
    {
        _savedData = spikeData;

        if (_savedData != null)
        {
            if (!beingDragged)
                LoadTransformData(ref _savedData);
        }
    }

    public TransformData GetSavedData()
    {
        if (_savedData != null)
            return _savedData;

        return null;
    }
}