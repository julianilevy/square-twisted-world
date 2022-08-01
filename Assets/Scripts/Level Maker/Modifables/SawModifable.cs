using System.Collections.Generic;

public class SawModifable : LMBasePrefabModifable
{
    private Saw _saw;

    protected override void Awake()
    {
        base.Awake();
        _saw = GetComponent<Saw>();
    }

    public override void AddBoolLists()
    {
        AddToBoolList("Cyclic", _saw.cyclic, false);
    }

    public override void AddFloatLists()
    {
        AddToFloatList("Speed", _saw.speed);
        AddToFloatList("Wait Time", _saw.waitTime);
        AddToFloatList("Start Time", _saw.startTime);
    }

    public override void AddEnumLists()
    {
        var rotatingDirectionList = new List<string>();
        rotatingDirectionList.Add(Saw.RotatingDirections.right);
        rotatingDirectionList.Add(Saw.RotatingDirections.left);
        var rotatingDirectionDependantFloatList = new List<Tuple<string, Ref<float>>>();
        rotatingDirectionDependantFloatList.Add(Tuple.Create("Rotation Speed", _saw.rotationSpeed));
        AddToEnumList("Rotation Direction", _saw.rotationDirection, rotatingDirectionList, rotatingDirectionDependantFloatList);
    }
}