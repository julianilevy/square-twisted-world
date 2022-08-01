using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class PlayerStats : MonoBehaviour
{
    public int health = 1;
    public float damageCooldown = 1f;

    [HideInInspector]
    public FullEntityMotor carryingObject;
    [HideInInspector]
    public bool isCarryingObject;
    [HideInInspector]
    public bool dead;

    private Player _player;
    private float _damageCooldown;
    private float _respawnTimer;

    void Awake()
    {
        _player = GetComponent<Player>();
    }

    void Update()
    {
        if (GameManager.instance.IsPlayingLevel() && GameManager.instance.isAbleToPlay)
        {
            CarryObject();
            DamageCooldown();
            Die();
            Respawn();
            CheckGrabObjectAlert(); // NEGRADA...
        }
    }

    void CarryObject()
    {
        if (isCarryingObject)
        {
            if (carryingObject != null)
            {
                carryingObject.transform.position = transform.position;
                carryingObject.transform.rotation = transform.rotation;
                carryingObject.previousGravity = _player.previousGravity;
                carryingObject.currentGravity = _player.currentGravity;
            }
        }
    }

    void DamageCooldown()
    {
        _damageCooldown -= Time.deltaTime;
    }

    void Die()
    {
        if (dead)
            DeadStatus();
    }

    void Respawn()
    {
        if (!dead)
        {
            if (health <= 0)
            {
                MasterAudio.PlaySound3DFollowTransformAndForget("Lose", transform);
                dead = true;
            }
        }

        if (dead)
        {
            _respawnTimer += Time.deltaTime;

            if (_respawnTimer >= 0.3f)
            {
                GameManager.fadeTime = 0.5f;
                CanvasUIManager.instance.EnableFade(CanvasUIManager.instance.noiseFade);
                StartCoroutine(ExecuteRespawn());
            }
        }
    }

    IEnumerator ExecuteRespawn()
    {
        yield return new WaitForSeconds(GameManager.fadeTime);

        GameManager.instance.deathCounter++;
        GameManager.instance.RestartLevel();
    }

    public void GetDamage(int damage)
    {
        if (_damageCooldown <= 0)
        {
            health--;
            _damageCooldown = damageCooldown;
        }
    }

    public void PickObject()
    {
        if (!isCarryingObject)
        {
            carryingObject = GetCarryingObject();

            if (carryingObject != null)
            {
                carryingObject.GetGrabbedByPlayer();
                isCarryingObject = true;
            }
        }
    }

    public void DropObject()
    {
        if (isCarryingObject)
        {
            if (carryingObject != null)
            {
                carryingObject.GetReleasedFromPlayer();
                isCarryingObject = false;
                carryingObject = null;
            }
        }
    }

    void DeadStatus()
    {
        DropObject();
        _player.locked = true;
        _player.rotating = false;
        _player.currentSpeed = 0;
        _player.velocity = Vector2.zero;
        _player.GetComponent<Collider2D>().enabled = false;
        _player.sprite.SetActive(false);
        _player.sprite.PlayDeathParticles();
        _player.sprite.currentGravityParticles.Stop();
        _player.sprite.startRunningParticles.Stop();
    }

    FullEntityMotor GetCarryingObject()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        var topLeft = new Vector2(bounds.min.x, bounds.max.y);
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        var hit = Physics2D.OverlapArea(topLeft, bottomRight, _player.onlyHandheldColliderMask);

        if (hit != null)
        {
            var gravityEntity = hit.GetComponentInParent<FullEntityMotor>();
            return gravityEntity;
        }

        return null;
    }

    // NEGRADA...

    void CheckGrabObjectAlert()
    {
        if (!isCarryingObject)
        {
            var bounds = GetComponent<BoxCollider2D>().bounds;
            var topLeft = new Vector2(bounds.min.x, bounds.max.y);
            var bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            var hits = Physics2D.OverlapAreaAll(topLeft, bottomRight, _player.onlyHandheldColliderMask);
            var gravityEntityCount = 0;

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                    gravityEntityCount++;
                if (gravityEntityCount > 0)
                {
                    if (CanvasUIManager.instance.grabObjectAlert != null)
                        CanvasUIManager.instance.grabObjectAlert.SetActive(true);
                }
                else
                {
                    if (CanvasUIManager.instance.grabObjectAlert != null)
                        CanvasUIManager.instance.grabObjectAlert.SetActive(false);
                }
            }
            else
            {
                if (CanvasUIManager.instance.grabObjectAlert != null)
                    CanvasUIManager.instance.grabObjectAlert.SetActive(false);
            }
        }
        else
        {
            if (CanvasUIManager.instance.grabObjectAlert != null)
                CanvasUIManager.instance.grabObjectAlert.SetActive(false);
        }
    }
}