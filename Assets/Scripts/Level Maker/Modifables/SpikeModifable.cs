public class SpikeModifable : LMBasePrefabModifable
{
    private Spike _spike;

    protected override void Awake()
    {
        base.Awake();
        _spike = GetComponent<Spike>();
    }
}