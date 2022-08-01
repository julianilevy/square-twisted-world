public abstract class GravityEntityData : TransformData
{
    public double[] currentGravity;

    public GravityEntityData()
    {
        currentGravity = new double[2];
    }

    public void SetCurrentGravityValues(double forceX, double forceY)
    {
        currentGravity[0] = forceX;
        currentGravity[1] = forceY;
    }
}