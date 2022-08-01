public class PassableTileModifable : LMBasePrefabModifable
{
    private PassableTile _passableTile;

    protected override void Awake()
    {
        base.Awake();
        _passableTile = GetComponent<PassableTile>();
    }
}