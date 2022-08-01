using UnityEngine;

public class Rotation
{
    // 01 es la rotación para girar en el sentido de las agujas del reloj.
    // 02 es la rotación para girar en el sentido contrario al de las agujas del reloj.

    public static Quaternion UP01 = new Quaternion(0f, 0f, -1f, 0f);
    public static Quaternion UP02 = new Quaternion(0f, 0f, 1f, 0f);
    public static Quaternion RIGHT01 = new Quaternion(0f, 0f, -0.7f, -0.7f);
    public static Quaternion RIGHT02 = new Quaternion(0f, 0f, 0.7f, 0.7f);
    public static Quaternion LEFT01 = new Quaternion(0f, 0f, -0.7f, 0.7f);
    public static Quaternion LEFT02 = new Quaternion(0f, 0f, 0.7f, -0.7f);
    public static Quaternion DOWN01 = new Quaternion(0f, 0f, 0f, 1f);
    public static Quaternion DOWN02 = new Quaternion(0f, 0f, 0f, -1f);

    public static Vector3 EULER_UP = new Vector3(0f, 0f, 180f);
    public static Vector3 EULER_RIGHT = new Vector3(0f, 0f, 90f);
    public static Vector3 EULER_LEFT = new Vector3(0f, 0f, 270f);
    public static Vector3 EULER_DOWN = new Vector3(0f, 0f, 0f);
}
