using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : NetworkBehaviour
{
    private SpriteRenderer renderer;
    private Rigidbody2D rb;
    private Spellcasting spellcasting;
    private TrailRenderer trailRenderer;
    private GameObject currentTrail;
    Vector2 direction;

    [Header("Bullet Stats")]
    [SerializeField]
    private LayerMask whatDestroysBullet;
    [SerializeField]
    private LayerMask bulletPlayerCollision;
    [SerializeField]
    private LayerMask otherBulletsCollision;
    [SerializeField]
    public GameObject trailPrefab;


    //these stats are in BulletStats
    private BulletStats stats;

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
    private float _bounceEscapeTime;

    private float currentSizeMod;
    private float currentSpeedMod;
    private float currentDamageMod;

    private float currentDamage;
    private float currentHealth;

    public enum BulletType
    {
        Normal, //straight velocity, no gravity
        Physics, //affected by gravity, rotates in direction of velocity
        Trail, //leaves a trail behind it
        BounceOnWall, //bounces off surfaces a certain amount of times before being destroyed
        IncreaseSizeOnBounce, //when combined with Bounce, increases size of bullet on each bounce
        Split, //on collision, spawns multiple smaller bullets that fly in different directions
        Explosion, //on codestruction, spawns an explosion that damages everything in a radius
        DamageScaleWithSize,
        DamageScaleWithSpeed,
        KnockBackFromBullet,
        KnockBackFromPlayer,
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();

        currentTrail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        currentTrail.transform.SetParent(transform);
        trailRenderer = currentTrail.GetComponent<TrailRenderer>();
    }

    void OnEnable()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }


    private void FixedUpdate()
    {
        if (rb == null) { return ; }
        if (Time.time > _disableTime)
            DestroyBullet();

        currentSpeedMod = (float)(Math.Pow(rb.linearVelocity.magnitude / stats.bulletSpeed / normalBulletSpeed, 1.3));

        //set Damage
        currentDamage = stats.bulletDamage * currentDamageMod 
            * (stats.bulletTypes.Contains(BulletType.DamageScaleWithSize) ? currentSizeMod : 1f)
            * (stats.bulletTypes.Contains(BulletType.DamageScaleWithSpeed) ? currentSpeedMod : 1f);

        


        if (Time.time > _bounceEscapeTime)
        {
            this.transform.localScale = Vector3.one * stats.bulletSize * currentSizeMod;
        }

        if (stats.bulletTypes.Contains(BulletType.Physics))
        {
            //rotate bullet in direction of velocity;
            transform.right = rb.linearVelocity;
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -90);
        }

        //Collision
        Vector2 velocity = rb.linearVelocity;
        float travelDistance = velocity.magnitude * Time.fixedDeltaTime;
        Vector2 direction = velocity.normalized;

        if (travelDistance <= 0) return;

        RaycastHit2D hit;
       

        hit = Physics2D.CircleCast(transform.position, this.transform.localScale.magnitude * 0.4f, direction, travelDistance, otherBulletsCollision);
        if (hit.collider != null && hit.collider.attachedRigidbody != rb && isServer)
        {
            Collider2D collision = hit.collider;

            RaycastHit2D terrainCheck = Physics2D.Linecast(transform.position, hit.centroid, whatDestroysBullet);
            if (terrainCheck.collider == null)
            {
                BulletCollision(collision);
            }
        }


        hit = Physics2D.CircleCast(transform.position, this.transform.localScale.magnitude * 0.4f, direction, travelDistance, bulletPlayerCollision);
        if (hit.collider != null && isServer)
        {
            Collider2D collision = hit.collider;

            RaycastHit2D terrainCheck = Physics2D.Linecast(transform.position, hit.centroid, whatDestroysBullet);
            if (terrainCheck.collider == null)
            {
                PlayerCollision(collision);
            }
        }


        hit = Physics2D.CircleCast(transform.position, this.transform.localScale.magnitude * 0.4f, direction, travelDistance, whatDestroysBullet);
        if (hit.collider != null)
        {
            if (stats.bulletTypes.Contains(BulletType.BounceOnWall) && stats.bounces > 0)
            {
                stats.bounces--;

                Vector2 newDirection = Vector2.Reflect(direction, hit.normal);
                transform.right = newDirection;
                float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle -90);
                rb.linearVelocity = newDirection * velocity.magnitude * stats.bounciness;

                if (stats.bulletTypes.Contains(BulletType.IncreaseSizeOnBounce))
                {
                    currentSizeMod += velocity.magnitude * stats.growthMod;
                    //currentHealth +=  stats.bulletHealth * (velocity.magnitude * stats.growthMod);
                    _bounceEscapeTime = Time.time + 0.05f;
                }
            }
            else
            {
                DestroyBullet();
                Debug.Log("hit Wall");
            }
        }
    }

    private void BulletCollision(Collider2D collision)
    {
        if (!isServer) return;
        Bullet bullet = collision.GetComponent<Bullet>();
        Debug.Log(bullet);
        if (stats.owner != bullet.stats.owner)
        {
            if (bullet != null)
            {
                bullet.currentHealth -= currentDamage;
                this.currentHealth -= bullet.currentDamage;

                Debug.Log("Other Bullet: " + bullet.currentHealth);
                Debug.Log("This Bullet: " + currentHealth);

                if (bullet.currentHealth <= 0)
                {
                    bullet.DestroyBullet();
                }
                if (this.currentHealth <= 0)
                {
                    DestroyBullet();
                }
            }
        }
    }

    private void PlayerCollision(Collider2D collision)
    {
        if (!isServer) return;
        if (collision.gameObject == stats.owner && _escapeTime > Time.time) { return; } //attempt to fix problem where the bullet hits the player immediately whem shooting

        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage((int)currentDamage);
            if (stats.bulletTypes.Contains(BulletType.KnockBackFromBullet))
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                KnockbackServer(knockbackDirection, health);
                KnockbackRpc(knockbackDirection, health);
            }

            if (stats.bulletTypes.Contains(BulletType.KnockBackFromPlayer))
            {
                Vector2 knockbackDirection = (transform.position - stats.owner.transform.position).normalized;
                KnockbackServer(knockbackDirection, health);
                KnockbackRpc(knockbackDirection, health);
            }
        }
        //Screen shake
        DestroyBullet();
        Debug.Log("hit Player");
    }

    [Server]
    public void LaunchServer(Vector2 direction, Quaternion rotation, Vector2 position, BulletStats stats)
    {
        this.stats = stats;
        renderer.material.color = stats.bulletColor;
        renderer.sprite = SpriteLibrary.instance.GetSprite(stats.sprite);
        this.transform.localScale = Vector3.one * stats.bulletSize;
        this.direction = direction;

        transform.SetPositionAndRotation(position, rotation * Quaternion.Euler(0, 0, -90));
        gameObject.SetActive(true);
        InitializeBulletType();
        InitializeBullet();
    }

    [ClientRpc]
    public void RpcLaunch(Vector2 direction, Quaternion rotation, Vector2 position, BulletStats stats)
    {
        // In host mode, the server already initialized the bullet.
        if (isServer) return;
        this.stats = stats;
        renderer.material.color = stats.bulletColor;
        renderer.sprite = SpriteLibrary.instance.GetSprite(stats.sprite);
        this.transform.localScale = Vector3.one  * stats.bulletSize;
        this.direction = direction;

        transform.SetPositionAndRotation(position, rotation * Quaternion.Euler(0, 0, -90));
        gameObject.SetActive(true);
        InitializeBulletType();
        InitializeBullet();
    }


    private void InitializeBullet()
    {
        _disableTime = Time.time + stats.timeToLive;
        _escapeTime = Time.time + stats.timeToEscape;

        currentSizeMod = 1f;
        currentSpeedMod = 1f;
        currentDamageMod = 1f;

        currentHealth = stats.bulletHealth;

        spellcasting = stats.owner.GetComponent<Spellcasting>();
    }

    public void Cast(Vector2 direction, Quaternion rotation, Vector2 position, BulletStats stats)
    {
        LaunchServer(direction, rotation, position, stats);
        RpcLaunch(direction, rotation, position, stats);
    }

    private void InitializeBulletType()
    {
        if (stats.bulletTypes.Contains(BulletType.Normal))
        {
            SetStraightVelocity();
            rb.gravityScale = 0;
        }
        if (stats.bulletTypes.Contains(BulletType.Physics))
        {
            SetPhysicsVelocity();
            rb.gravityScale = physicsBulletGravity;
        }
        trailRenderer.enabled = false;
        if (stats.bulletTypes.Contains(BulletType.Trail)){
            trailRenderer.Clear();
            trailRenderer.enabled = true;
            trailRenderer.widthMultiplier = stats.bulletSize;
            trailRenderer.time = stats.trailLength;
            trailRenderer.material.color = renderer.material.color;
        }
    }

    private void SetStraightVelocity()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * normalBulletSpeed * stats.bulletSpeed;
    }

    private void SetPhysicsVelocity()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * physicsBulletSpeed * stats.bulletSpeed;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return;

        if ((otherBulletsCollision.value & (1 << collision.gameObject.layer)) > 0)
        {
            BulletCollision(collision);
        }

        if ((bulletPlayerCollision.value & (1 << collision.gameObject.layer)) > 0)
        {
            PlayerCollision(collision);
        }
    }
    

    [Server]
    private void KnockbackServer(Vector2 knockbackDirection, Health health)
    {
        Knockback(knockbackDirection, health);
    }

    [ClientRpc]
    private void KnockbackRpc(Vector2 knockbackDirection, Health health)
    {
        Knockback(knockbackDirection, health);
    }

    private void Knockback(Vector2 knockbackDirection, Health health)
    {
        PlayerMovementController playerMovement = health.GetComponent<PlayerMovementController>();
        knockbackDirection = knockbackDirection * (float)(stats.knockbackForce * Math.Pow(currentSizeMod, 1.3));
        knockbackDirection = new Vector2(knockbackDirection.x * 2, (knockbackDirection.y * 0.5f));
        playerMovement.Knockback(knockbackDirection);
    }

    
    private void DestroyBullet()
    {
        if (!isServer) {  return; }
        RpcDestroyBullet();
        ServerDestroyBullet();
    }

    [ClientRpc]
    private void RpcDestroyBullet()
    {
        DestroyBulletBase();
    }

    [Server]
    private void ServerDestroyBullet()
    {
        DestroyBulletBase();
    }

    private void DestroyBulletBase()
    {
        if (!isServer) return;
        if (stats.bulletTypes.Contains(BulletType.Split))
        {
            RpcSplitBullet();
            //ServerSplitBullet();
        }
        if (stats.bulletTypes.Contains(BulletType.Explosion))
        {
            spellcasting.Explosion(this.transform.position, this.stats.explosionRadius, this.stats.explosionDamage, this.stats.explosionDamageMultMaxRange);
        }
        if (stats.bulletTypes.Contains(BulletType.Trail))
        {
            DetachAndSwapTrail();
        }
        ReturnToPoolServer();
    }

    [ClientRpc]
    private void RpcSplitBullet()
    {
        SplitBullet();
    }

    [Server]
    private void ServerSplitBullet()
    {
        SplitBullet();
    }

    private void SplitBullet()
    {
        foreach (BulletStats bullet in stats.splitBullets)
        {
            Vector2 castDirection = Quaternion.Euler(0, 0, bullet.splitAngleOffset) * this.transform.right;

            Quaternion castAngle = Quaternion.Euler(0, 0, Mathf.Atan2(castDirection.y, castDirection.x) * Mathf.Rad2Deg);

            spellcasting.CastBullet(castDirection, this.transform.position, castAngle, bullet);
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

    private void DetachAndSwapTrail()
    {
        trailRenderer.transform.SetParent(null);
        trailRenderer.emitting = false;
        Destroy(trailRenderer.gameObject, trailRenderer.time);

        currentTrail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        currentTrail.transform.SetParent(transform);
        trailRenderer = currentTrail.GetComponent<TrailRenderer>();
    }
}
