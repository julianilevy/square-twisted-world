public abstract class MobileBaseData : TransformData
{
    public WaypointData[] waypointData;
    public double speed;
    public double waitTime;
    public double startTime;
    public bool cyclic;

    public MobileBaseData()
    {
        waypointData = new WaypointData[0];
    }

    public void SetWaypointsData(WaypointData[] waypointData)
    {
        this.waypointData = waypointData;
    }

    public void SetMobileValues(double speed, double waitTime, double startTime, bool cyclic)
    {
        this.speed = speed;
        this.waitTime = waitTime;
        this.startTime = startTime;
        this.cyclic = cyclic;
    }
}