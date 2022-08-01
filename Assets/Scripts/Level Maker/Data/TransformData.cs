public abstract class TransformData
{
    public double[] position;
    public double[] rotation;

    public void SetPositionValues(double positionX, double positionY, double positionZ)
    {
        position = new double[3];
        position[0] = positionX;
        position[1] = positionY;
        position[2] = positionZ;
    }

    public void SetRotationValues(double rotationX, double rotationY, double rotationZ, double rotationW)
    {
        rotation = new double[4];
        rotation[0] = rotationX;
        rotation[1] = rotationY;
        rotation[2] = rotationZ;
        rotation[3] = rotationW;
    }
}