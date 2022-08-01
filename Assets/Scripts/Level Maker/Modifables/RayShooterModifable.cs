using System.Collections.Generic;

public class RayShooterModifable : LMBasePrefabModifable
{
    private RayShooter _rayShooter;

    protected override void Awake()
    {
        base.Awake();
        _rayShooter = GetComponent<RayShooter>();
    }

    public override void AddBoolLists()
    {
        var rayUpDependantFloatList = new List<Tuple<string, Ref<float>>>();
        rayUpDependantFloatList.Add(Tuple.Create("Intermittence", _rayShooter.rayUpIntervalIntermittenceTime));
        rayUpDependantFloatList.Add(Tuple.Create("Duration", _rayShooter.rayUpIntervalDurationTime));
        AddToBoolList("Ray Up", _rayShooter.rayUp, true, rayUpDependantFloatList);

        var rayRightDependantFloatList = new List<Tuple<string, Ref<float>>>();
        rayRightDependantFloatList.Add(Tuple.Create("Intermittence", _rayShooter.rayRightIntervalIntermittenceTime));
        rayRightDependantFloatList.Add(Tuple.Create("Duration", _rayShooter.rayRightIntervalDurationTime));
        AddToBoolList("Ray Right", _rayShooter.rayRight, true, rayRightDependantFloatList);

        var rayLeftDependantFloatList = new List<Tuple<string, Ref<float>>>();
        rayLeftDependantFloatList.Add(Tuple.Create("Intermittence", _rayShooter.rayLeftIntervalIntermittenceTime));
        rayLeftDependantFloatList.Add(Tuple.Create("Duration", _rayShooter.rayLeftIntervalDurationTime));
        AddToBoolList("Ray Left", _rayShooter.rayLeft, true, rayLeftDependantFloatList);

        var rayDownDependantFloatList = new List<Tuple<string, Ref<float>>>();
        rayDownDependantFloatList.Add(Tuple.Create("Intermittence", _rayShooter.rayDownIntervalIntermittenceTime));
        rayDownDependantFloatList.Add(Tuple.Create("Duration", _rayShooter.rayDownIntervalDurationTime));
        AddToBoolList("Ray Down", _rayShooter.rayDown, true, rayDownDependantFloatList);

        AddToBoolList("Cyclic", _rayShooter.cyclic, false);
    }

    public override void AddFloatLists()
    {
        AddToFloatList("Speed", _rayShooter.speed);
        AddToFloatList("Wait Time", _rayShooter.waitTime);
        AddToFloatList("Start Time", _rayShooter.startTime);
    }

    public override void AddEnumLists()
    {
        var rotatingDirectionList = new List<string>();
        rotatingDirectionList.Add(RayShooter.RotatingDirections.right);
        rotatingDirectionList.Add(RayShooter.RotatingDirections.left);
        var rotatingDirectionDependantFloatList = new List<Tuple<string, Ref<float>>>();
        rotatingDirectionDependantFloatList.Add(Tuple.Create("Rotation Speed", _rayShooter.rotationSpeed));
        AddToEnumList("Rotation Direction", _rayShooter.rotationDirection, rotatingDirectionList, rotatingDirectionDependantFloatList);
    }
}