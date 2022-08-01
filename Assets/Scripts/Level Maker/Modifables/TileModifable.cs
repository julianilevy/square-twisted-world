public class TileModifable : LMBasePrefabModifable
{
    private Tile _tile;

    protected override void Awake()
    {
        base.Awake();
        _tile = GetComponent<Tile>();
    }
}