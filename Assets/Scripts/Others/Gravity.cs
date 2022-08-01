using UnityEngine;

public class Gravity
{
    public static Vector2 UP = new Vector2(0, 1);
    public static Vector2 RIGHT = new Vector2(1, 0);
    public static Vector2 LEFT = new Vector2(-1, 0);
    public static Vector2 DOWN = new Vector2(0, -1);

    public static Color COLOR_UP = GetColor(235, 0, 235, 255);
    public static Color COLOR_RIGHT = GetColor(0, 255, 0, 255);
    public static Color COLOR_LEFT = GetColor(235, 235, 0, 255);
    public static Color COLOR_DOWN = GetColor(0, 235, 235, 255);

    public struct Forces
    {
        public static string up = "Up";
        public static int upIndex = 0;
        public static string right = "Right";
        public static int rightIndex = 1;
        public static string left = "Left";
        public static int leftIndex = 2;
        public static string down = "Down";
        public static int downIndex = 3;
    }

    public static Vector2 GetGravityByForces(string gravityForce)
    {
        if (gravityForce == Forces.up) return UP;
        if (gravityForce == Forces.right) return RIGHT;
        if (gravityForce == Forces.left) return LEFT;
        if (gravityForce == Forces.down) return DOWN;

        return Vector2.zero;
    }

    static Color GetColor(float r, float g, float b, float a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}