public abstract class BaseTileData : TransformData
{
    public bool destroyable;
    public bool alone;
    public bool emissionUpEnabled;
    public bool emissionRightEnabled;
    public bool emissionLeftEnabled;
    public bool emissionDownEnabled;

    public void SetBaseTileData(bool destroyable, bool alone)
    {
        this.destroyable = destroyable;
        this.alone = alone;
    }

    public void SetEmissionData(bool emissionUp, bool emissionRight, bool emissionLeft, bool emissionDown)
    {
        emissionUpEnabled = emissionUp;
        emissionRightEnabled = emissionRight;
        emissionLeftEnabled = emissionLeft;
        emissionDownEnabled = emissionDown;
    }
}