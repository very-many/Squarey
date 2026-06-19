using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

public class Spellcasting : NetworkBehaviour
{
    public GameObject bullet;

    public PlayerMovementController playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovementController>();
    }

    public void CastBullet(Vector2 castDirection, Vector2 castPosition, Quaternion targetRotation, BulletStats bulletStats)
    {
        CmdCastBullet(castDirection, castPosition, targetRotation, bulletStats);
    }

    [Command]
    public void CmdCastBullet(Vector2 castDirection, Vector2 castPosition, Quaternion targetRotation, BulletStats bulletStats)
    {
        GameObject go = null;
        if (ObjectPool.instance != null)
        {
            go = ObjectPool.instance.GetServerObject();
            if (go != null)
            {
                go.transform.SetPositionAndRotation(castPosition, targetRotation * Quaternion.Euler(0, 0, -90));
                go.SetActive(true);
                NetworkServer.Spawn(go);
            }
        }

        if (go == null)
        {
            go = Instantiate(bullet, castPosition, targetRotation * Quaternion.Euler(0, 0, -90));
            NetworkServer.Spawn(go);
        }

        var bulletScript = go.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Cast(castDirection, targetRotation, castPosition, bulletStats);
        }
    }

    public void Jump()
    {
        if (playerMovement == null) { return; }
        playerMovement.SpellJump(Vector2.up);
    }

    [Command]
    private void CmdJump()
    {
        if (playerMovement == null) { return; }
        playerMovement.SpellJump(Vector2.up);
    }

    public void Explosion(Vector2 explosionPosition, float explosionRadius, float explosionDamage, float explosionDamageMultMaxRange)
    {
        CmdExplosion(explosionPosition, explosionRadius, explosionDamage, explosionDamageMultMaxRange);
    }

    [Command]
    private void CmdExplosion(Vector2 explosionPosition, float explosionRadius, float explosionDamage, float explosionDamageMultMaxRange)
    {
        Debug.Log("Explosion is not implemented thus far... cough... eh... *BOOM*");
    }
}