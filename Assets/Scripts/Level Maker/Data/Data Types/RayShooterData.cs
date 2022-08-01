using System;

[Serializable]
public class RayShooterData : MobileBaseData
{
    public bool rayUp;
    public double rayUpIntervalIntermittenceTime;
    public double rayUpIntervalDurationTime;
    public bool rayRight;
    public double rayRightIntervalIntermittenceTime;
    public double rayRightIntervalDurationTime;
    public bool rayLeft;
    public double rayLeftIntervalIntermittenceTime;
    public double rayLeftIntervalDurationTime;
    public bool rayDown;
    public double rayDownIntervalIntermittenceTime;
    public double rayDownIntervalDurationTime;
    public string rotationDirectionName;
    public int rotationDirectionIndex;
    public double rotationSpeed;

    public RayShooterData() : base()
    {
    }

    public void SetRayUpValues(bool rayUp, double rayUpIntervalIntermittenceTime, double rayUpIntervalDurationTime)
    {
        this.rayUp = rayUp;
        this.rayUpIntervalIntermittenceTime = rayUpIntervalIntermittenceTime;
        this.rayUpIntervalDurationTime = rayUpIntervalDurationTime;
    }

    public void SetRayRightValues(bool rayRight, double rayRightIntervalIntermittenceTime, double rayRightIntervalDurationTime)
    {
        this.rayRight = rayRight;
        this.rayRightIntervalIntermittenceTime = rayRightIntervalIntermittenceTime;
        this.rayRightIntervalDurationTime = rayRightIntervalDurationTime;
    }

    public void SetRayLeftValues(bool rayLeft, double rayLeftIntervalIntermittenceTime, double rayLeftIntervalDurationTime)
    {
        this.rayLeft = rayLeft;
        this.rayLeftIntervalIntermittenceTime = rayLeftIntervalIntermittenceTime;
        this.rayLeftIntervalDurationTime = rayLeftIntervalDurationTime;
    }

    public void SetRayDownValues(bool rayDown, double rayDownIntervalIntermittenceTime, double rayDownIntervalDurationTime)
    {
        this.rayDown = rayDown;
        this.rayDownIntervalIntermittenceTime = rayDownIntervalIntermittenceTime;
        this.rayDownIntervalDurationTime = rayDownIntervalDurationTime;
    }

    public void SetRotationDirectionValues(string rotationDirectionName, int rotationDirectionIndex, double rotationSpeed)
    {
        this.rotationDirectionName = rotationDirectionName;
        this.rotationDirectionIndex = rotationDirectionIndex;
        this.rotationSpeed = rotationSpeed;
    }
}