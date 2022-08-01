public class LockedDoorModifable : LMBasePrefabModifable
{
    private LockedDoor _lockedDoor;

    protected override void Awake()
    {
        base.Awake();
        _lockedDoor = GetComponent<LockedDoor>();
    }
}