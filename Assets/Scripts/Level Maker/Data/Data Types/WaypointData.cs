using System;

[Serializable]
public class WaypointData : TransformData
{
    public int index;

    public void SetIndexValue(int index)
    {
        this.index = index;
    }
}