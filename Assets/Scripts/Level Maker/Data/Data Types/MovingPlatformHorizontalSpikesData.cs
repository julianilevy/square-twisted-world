using System;

[Serializable]
public class MovingPlatformHorizontalSpikesData : MobileBaseData
{
    public string platformType;

    public MovingPlatformHorizontalSpikesData() : base()
    {
    }

    public void SetPlatformType(string platformType)
    {
        this.platformType = platformType;
    }
}