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
    public void castBullet(Vector2 castDirection, Vector2 castPosition, Quaternion targetRotation, float damage, float health, float size, List<Bullet.BulletType> types)
    {
        if (Mirror.NetworkServer.active)
        {
            if (isOwned)
            {
                CmdSpawnBullet(castDirection, castPosition, targetRotation);
            }
        }
        else
        {
            LocalBulletSpawn(castDirection, castPosition, targetRotation, damage, health, size, types);
        }

    }

    [Command]
    private void CmdSpawnBullet(Vector2 castDirection, Vector2 castPosition, Quaternion targetRotation)
    {
        GameObject bulletInstance = Instantiate(bullet, castPosition, targetRotation);

        NetworkServer.Spawn(bulletInstance);
        var bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.LaunchServer(castDirection, targetRotation, castPosition);
            bulletScript.RpcLaunch(castDirection, targetRotation, castPosition);
        }
    }

    private void LocalBulletSpawn(Vector2 targetPosition, Vector2 castPosition, Quaternion castDirection, float damage, float health, float size, List<Bullet.BulletType> types)
    {
        GameObject bulletInstance = Instantiate(bullet, castPosition, castDirection);
        var bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.LocalCast(targetPosition, castDirection, castPosition, damage, health, size, types);
        }
    }
}
