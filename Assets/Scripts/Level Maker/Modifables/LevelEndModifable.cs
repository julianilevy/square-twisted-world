public class LevelEndModifable : LMBasePrefabModifable
{
    private LevelEnd _levelEnd;

    protected override void Awake()
    {
        base.Awake();
        _levelEnd = GetComponent<LevelEnd>();
    }
}