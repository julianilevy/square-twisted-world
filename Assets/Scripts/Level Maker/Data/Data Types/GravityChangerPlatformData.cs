using System;

[Serializable]
public class GravityChangerPlatformData : TransformData
{
    public string nextGravityName;
    public int nextGravityIndex;

    public void SetNextGravityValues(string nextGravityName, int nextGravityIndex)
    {
        this.nextGravityName = nextGravityName;
        this.nextGravityIndex = nextGravityIndex;
    }
}