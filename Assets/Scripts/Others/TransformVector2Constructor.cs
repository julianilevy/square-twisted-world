using UnityEngine;

public class TransformVector2Constructor
{
    public Vector2 position;
    public Vector2 localPosition;
    public Vector2 up;
    public Vector2 down;
    public Vector2 right;
    public Vector2 left;

    public TransformVector2Constructor(Vector3 _position, Vector3 _localPosition, Vector3 _up, Vector3 _down, Vector3 _right, Vector3 _left)
    {
        position = new Vector2(_position.x, _position.y);
        localPosition = new Vector2(_localPosition.x, _localPosition.y);
        up = new Vector2(_up.x, _up.y);
        down = new Vector2(_down.x, _down.y);
        right = new Vector2(_right.x, _right.y);
        left = new Vector2(_left.x, _left.y);
    }
}