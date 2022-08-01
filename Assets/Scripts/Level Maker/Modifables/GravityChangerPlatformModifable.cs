using System.Collections.Generic;

public class GravityChangerPlatformModifable : LMBasePrefabModifable
{
    private GravityChangerPlatform _gravityChangerPlatform;

    protected override void Awake()
    {
        base.Awake();
        _gravityChangerPlatform = GetComponent<GravityChangerPlatform>();
    }

    public override void AddEnumLists()
    {
        var gravityForcesList = new List<string>();
        gravityForcesList.Add(Gravity.Forces.up);
        gravityForcesList.Add(Gravity.Forces.right);
        gravityForcesList.Add(Gravity.Forces.left);
        gravityForcesList.Add(Gravity.Forces.down);
        AddToEnumList("Next Gravity", _gravityChangerPlatform.nextGravity, gravityForcesList);
    }
}