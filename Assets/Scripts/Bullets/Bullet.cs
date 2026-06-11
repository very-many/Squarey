using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : NetworkBehaviour
{

    private Rigidbody2D rb;
    Vector2 direction;

    [Header("Bullet Stats")]
    [SerializeField]
    private LayerMask whatDestroysBullet;
    [SerializeField]
    private LayerMask otherBulletsCollision;
    [SerializeField]
    private float timeToLive = 5;
    [SerializeField]
    private float timeToEscape = 0.2f;

    //these stats are in BulletStats
    private List<BulletType> bulletTypes = new List<BulletType>();
    private float bulletDamage;
    private float bulletHealth;
    private float bulletSize;
    private float bulletSpeed;

    private GameObject _owner;

    [Header("Normal Bullets")]
    [SerializeField]
    private float normalBulletSpeed;

    [Header("Physics Bullets")]
    [SerializeField]
    private float physicsBulletSpeed;
    [SerializeField]
    private float physicsBulletGravity;

    private float _disableTime;
    private float _escapeTime;

    public enum BulletType
    {
        Normal, //straight velocity, no gravity
        Physics, //affected by gravity, rotates in direction of velocity
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        _disableTime = Time.time + timeToLive;
        _escapeTime = Time.time + timeToEscape;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void Update()
    {
        if (!isServer) return;

        if (Time.time > _disableTime)
            ReturnToPoolServer();

        this.transform.localScale = Vector3.one * bulletSize;
    }

    private void FixedUpdate()
    {
        if (bulletTypes.Contains(BulletType.Physics))
        {
            //rotate bullet in direction of velocity;
            transform.right = rb.linearVelocity;
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -90);
        }
    }

    [Server]
    public void LaunchServer(Vector2 direction, Quaternion rotation, Vector2 position)
    {
        this.direction = direction;
        transform.SetPositionAndRotation(position, rotation * Quaternion.Euler(0, 0, -90));
        gameObject.SetActive(true);
        InitializeBulletType();
    }

    [ClientRpc]
    public void RpcLaunch(Vector2 direction, Quaternion rotation, Vector2 position)
    {
        // In host mode, the server already initialized the bullet.
        if (isServer) return;

        this.direction = direction;
        transform.SetPositionAndRotation(position, rotation * Quaternion.Euler(0, 0, -90));
        gameObject.SetActive(true);
        InitializeBulletType();
    }

    public void Cast(Vector2 direction, Quaternion rotation, Vector2 position, BulletStats stats)
    {
        ApplyStatsClient(stats);
        ApplyStatsServer(stats);

        LaunchServer(direction, rotation, position);
        RpcLaunch(direction, rotation, position);
    }

    [Server]
    private void ApplyStatsServer(BulletStats stats)
    {
        ApplyStats(stats);
    }

    [ClientRpc]
    private void ApplyStatsClient(BulletStats stats)
    {
        ApplyStats(stats);
    }

    private void ApplyStats(BulletStats stats)
    {
        this.bulletTypes = stats.bulletTypes;
        this.bulletDamage = stats.bulletDamage;
        this.bulletHealth = stats.bulletHealth;
        this.bulletSize = stats.bulletSize;
        this.bulletSpeed = stats.bulletSpeed;
        //this.bulletColor = stats.bulletColor;
        this._owner = stats.owner;
    }

    private void InitializeBulletType()
    {
        if (bulletTypes.Contains(BulletType.Normal))
        {
            SetStraightVelocity();
            rb.gravityScale = 0;
        }
        if (bulletTypes.Contains(BulletType.Physics))
        {
            SetPhysicsVelocity();
            rb.gravityScale = physicsBulletGravity;
        }
    }

    private void SetStraightVelocity()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * normalBulletSpeed * bulletSpeed;
    }

    private void SetPhysicsVelocity()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * physicsBulletSpeed * bulletSpeed;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return;

        //collision is within layermask
        if ((whatDestroysBullet.value & (1 << collision.gameObject.layer)) > 0)
        {
            //spawn Paritcles
            //Soundeffect

            if (collision.gameObject == _owner && _escapeTime > Time.time) { return; } //attempt to fix problem where the bullet hits the player immediately whem shooting

            Health health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage((int)bulletDamage);
            }
            //Screen shake
            ReturnToPoolServer();
            Debug.Log("hit");
        }

        if ((otherBulletsCollision.value & (1 << collision.gameObject.layer)) > 0)
        {

            //spawn Paritcles
            //Soundeffect

            Bullet bullet = collision.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.bulletHealth -= bulletDamage;
                this.bulletHealth -= bullet.bulletDamage;
                if (bullet.bulletHealth <= 0)
                {
                    bullet.ReturnToPoolServer();
                }
                if (this.bulletHealth <= 0)
                {
                    ReturnToPoolServer();
                }
            }
        }
    }

    [Server]
    private void ReturnToPoolServer()
    {
        // prevent double-return (TTL + collision in same frame, etc.)
        if (!isServer || !gameObject.activeSelf) return;

        // UnSpawn tells clients to remove it. With a registered UnspawnHandler,
        // clients will return it to their local pool instead of Destroy().
        NetworkServer.UnSpawn(gameObject);

        // In dedicated-server mode, also keep a server-side reference.
        // In host mode, Mirror already invoked the client-side unspawn handler.
        if (!isClient && ObjectPool.instance != null)
            ObjectPool.instance.ReturnServerObject(gameObject);
    }
}
