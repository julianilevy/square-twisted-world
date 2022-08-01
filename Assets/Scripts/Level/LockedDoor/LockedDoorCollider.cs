using UnityEngine;

public class LockedDoorCollider : MonoBehaviour
{
    public LockedDoor lockedDoor;
    public Side side;

    [HideInInspector]
    public bool unlocking;

    public enum Side
    {
        Up,
        Right,
        Left,
        Down
    }

    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (!unlocking)
        {
            if (collider2D.transform.gameObject.layer == K.LAYER_PLAYER)
            {
                var player = collider2D.GetComponent<Player>();

                if (player.stats.isCarryingObject)
                {
                    if (player.stats.carryingObject.GetComponent<Key>())
                    {
                        player.stats.isCarryingObject = false;
                        Destroy(player.stats.carryingObject.gameObject);
                        player.stats.carryingObject = null;

                        lockedDoor.OnKeyFitted(side);
                    }
                }
            }
        }
    }
}