using System;

[Serializable]
public class MovingPlatformVerticalSpikesData : MobileBaseData
{
    public string platformType;

    public MovingPlatformVerticalSpikesData() : base()
    {
    }

    public void SetPlatformType(string platformType)
    {
        this.platformType = platformType;
    }
}