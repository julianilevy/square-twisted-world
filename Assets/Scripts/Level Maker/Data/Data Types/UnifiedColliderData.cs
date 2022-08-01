using System;

[Serializable]
public class UnifiedColliderData : TransformData
{
    public double[] offset;
    public double[] size;
    public string tag;
    public int layer;

    public void SetOffsetValues(double offsetX, double offsetY)
    {
        offset = new double[2];
        offset[0] = offsetX;
        offset[1] = offsetY;
    }

    public void SetSizeValues(double sizeX, double sizeY)
    {
        size = new double[2];
        size[0] = sizeX;
        size[1] = sizeY;
    }

    public void SetTagAndLayer(string tag, int layer)
    {
        this.tag = tag;
        this.layer = layer;
    }
}