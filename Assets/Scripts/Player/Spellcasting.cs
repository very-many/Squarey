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

    public void CastBullet(Vector2 castDirection, Vector2 castPosition, Quaternion targetRotation, float damage, float health, float size, List<Bullet.BulletType> types)
    {
        CmdCastBullet(castDirection, castPosition, targetRotation, damage, health, size, types);
    }

    [Command]
    public void CmdCastBullet(Vector2 castDirection, Vector2 castPosition, Quaternion targetRotation, float damage, float health, float size, List<Bullet.BulletType> types)
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
            bulletScript.Cast(castDirection, targetRotation, castPosition, damage, health, size, types);
        }
    }
}