public class PlayerModifable : LMBasePrefabModifable
{
    private Player _player;

    protected override void Awake()
    {
        base.Awake();
        _player = GetComponent<Player>();
    }
}