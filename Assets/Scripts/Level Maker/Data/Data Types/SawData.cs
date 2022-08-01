using System;

[Serializable]
public class SawData : MobileBaseData
{
    public string rotationDirectionName;
    public int rotationDirectionIndex;
    public double rotationSpeed;

    public SawData() : base()
    {
    }

    public void SetRotationDirectionValues(string rotationDirectionName, int rotationDirectionIndex, double rotationSpeed)
    {
        this.rotationDirectionName = rotationDirectionName;
        this.rotationDirectionIndex = rotationDirectionIndex;
        this.rotationSpeed = rotationSpeed;
    }
}