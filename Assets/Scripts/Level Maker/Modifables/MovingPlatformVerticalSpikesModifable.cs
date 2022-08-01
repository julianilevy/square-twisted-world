public class MovingPlatformVerticalSpikesModifable : LMBasePrefabModifable
{
    private MovingPlatformVerticalSpikes _movingPlatform;

    protected override void Awake()
    {
        base.Awake();
        _movingPlatform = GetComponent<MovingPlatformVerticalSpikes>();
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