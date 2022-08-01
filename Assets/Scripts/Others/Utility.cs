using UnityEngine;

public class Utility
{
    public static Vector2 ToVector2(Vector3 vector3)
    {
        var newVector2 = new Vector2(vector3.x, vector3.y);
        return newVector2;
    }

    public static void CopyCamera(ref Camera fromCamera, Camera toCamera)
    {
        fromCamera.orthographicSize = toCamera.orthographicSize;
    }
}