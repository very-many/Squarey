using Mirror;
using System.Collections;
using System.Collections.Generic;
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
    private int timeToLive = 5;
    public List<BulletType> bulletTypes;
    [SerializeField]
    private float bulletDamage = 10;
    [SerializeField]
    private float bulletHealth = 1;
    [SerializeField]
    private float bulletSize = 1;
    [SerializeField]
    private float bulletSpeed = 1;

    [Header("Normal Bullets")]
    [SerializeField]
    private float normalBulletSpeed;

    [Header("Physics Bullets")]
    [SerializeField]
    private float physicsBulletSpeed;
    [SerializeField]
    private float physicsBulletGravity;

    private float _disableTime;

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

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void Update()
    {
        if (!isServer) return;

        if (Time.time > _disableTime)
            ReturnToPoolServer();
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
        InitializeBulletStats();
    }

    [ClientRpc]
    public void RpcLaunch(Vector2 direction, Quaternion rotation, Vector2 position)
    {
        // In host mode, the server already initialized the bullet.
        if (isServer) return;

        this.direction = direction;
        transform.SetPositionAndRotation(position, rotation * Quaternion.Euler(0, 0, -90));
        gameObject.SetActive(true);
        InitializeBulletStats();
    }

    public void LocalCast(Vector2 direction, Quaternion rotation, Vector2 position, float damage, float health, float size, List<BulletType> types)
    {
        this.direction = direction;
        transform.SetPositionAndRotation(position, rotation * Quaternion.Euler(0, 0, -90));
        gameObject.SetActive(true);

        this.bulletDamage = damage;
        this.bulletHealth = health;
        this.bulletSize = size;
        this.bulletTypes = types;

        InitializeBulletStats();
    }

    private void InitializeBulletStats()
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
            //damage enemy
            Health health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage((int)bulletDamage);
            }
            //Screen shake
            ReturnToPoolServer();
            Debug.Log("hit");
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
