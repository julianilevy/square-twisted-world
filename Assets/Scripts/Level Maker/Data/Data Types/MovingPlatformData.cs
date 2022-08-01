using System;

[Serializable]
public class MovingPlatformData : MobileBaseData
{
    public string platformType;

    public MovingPlatformData() : base()
    {
    }

    public void SetPlatformType(string platformType)
    {
        this.platformType = platformType;
    }
}