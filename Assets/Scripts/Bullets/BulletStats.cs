using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class BulletStats
{
    public List<BulletType> bulletTypes = new List<BulletType>() { BulletType.Normal };

    public float bulletDamage = 10;
    public float bulletHealth = 1;
    public float bulletSize = 1;
    public float bulletSpeed = 1;

    public Color bulletColor = Color.red;

    public GameObject owner;

    //public float bulletScaleX = 1;
    //public float bulletScaleY = 1;

    //public float timeToLive = 3;

    //public float timeToEscape = 0,2;

    //public imagesrc = ???;

    //public hitbox = ???;

    public BulletStats() { }

    public BulletStats(List<BulletType> bulletTypes, float bulletDamage, float bulletHealth, float bulletSize, float bulletSpeed, Color bulletColor, GameObject owner)
    {
        this.bulletTypes = bulletTypes;
        this.bulletDamage = bulletDamage;
        this.bulletHealth = bulletHealth;
        this.bulletSize = bulletSize;
        this.bulletSpeed = bulletSpeed;
        this.bulletColor = bulletColor;
        this.owner = owner;
    }
}
