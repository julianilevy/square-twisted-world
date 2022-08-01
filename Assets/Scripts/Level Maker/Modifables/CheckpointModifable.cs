public class CheckpointModifable : LMBasePrefabModifable
{
    private Checkpoint _checkpoint;

    protected override void Awake()
    {
        base.Awake();
        _checkpoint = GetComponent<Checkpoint>();
    }
}