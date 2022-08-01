using UnityEngine;

public abstract class EnemyBase : BasePrefab
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.transform.gameObject.layer == K.LAYER_PLAYER)
        {
            var player = collider2D.GetComponent<Player>();
            player.GetDamage(damage);
        }
    }
}