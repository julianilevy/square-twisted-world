public class MovingPlatformHorizontalSpikesModifable : LMBasePrefabModifable
{
    private MovingPlatformHorizontalSpikes _movingPlatform;

    protected override void Awake()
    {
        base.Awake();
        _movingPlatform = GetComponent<MovingPlatformHorizontalSpikes>();
    }

    public override void AddBoolLists()
    {
        AddToBoolList("Cyclic", _movingPlatform.cyclic, false);
    }

    public override void AddFloatLists()
    {
        AddToFloatList("Speed", _movingPlatform.speed);
        AddToFloatList("Wait Time", _movingPlatform.waitTime);
        AddToFloatList("Start Time", _movingPlatform.startTime);
    }
}