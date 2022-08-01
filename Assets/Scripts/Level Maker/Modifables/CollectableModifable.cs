public class CollectableModifable : LMBasePrefabModifable
{
    private Collectable _collectable;

    protected override void Awake()
    {
        base.Awake();
        _collectable = GetComponent<Collectable>();
    }
}