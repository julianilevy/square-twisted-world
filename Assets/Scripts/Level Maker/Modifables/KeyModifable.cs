public class KeyModifable : LMBasePrefabModifable
{
    private Key _key;

    protected override void Awake()
    {
        base.Awake();
        _key = GetComponent<Key>();
    }
}