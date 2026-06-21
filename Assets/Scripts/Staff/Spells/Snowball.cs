using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static Bullet;

public class Snowball : Spell
{
    public string spellTitle => "Snowball";

    public float spellRecoveryTime => 3.5f;

    public float spellCastTime => 0.4f;

    public string spellImagePath => "Spells/Snowball";

    public int probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        List<BulletType> bulletTypes = new List<BulletType> { BulletType.Physics, BulletType.Trail, BulletType.BounceOnWall, BulletType.IncreaseSizeOnBounce, BulletType.DamageScaleWithSize, BulletType.KnockBackFromBullet };
        float bulletDamage = 0.7f * multiStaff.bulletDamageMult * multiStaff.ProjectileSize * multiStaff.MagicPower;
        float bulletHealth = 0.5f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float bulletSize = 0.15f * multiStaff.ProjectileSize;
        float bulletSpeed = 1.2f * multiStaff.ProjectileSpeed;

        DirectStaff directionalInfo = multiStaff.directionalInfo;

        BulletStats bulletStats = new BulletStats(bulletTypes, bulletDamage, bulletHealth, bulletSize, bulletSpeed, Color.white, multiStaff.player);

        bulletStats.trailLength = 0.04f;
        bulletStats.bounces = 50;
        bulletStats.bounciness = 0.875f;
        bulletStats.growthMod = 0.05f * multiStaff.ProjectileSize;
        bulletStats.sprite = SpriteLibrary.SpriteType.Circle;

        multiStaff.spellcasting.CastBullet(directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle, bulletStats);
    }
}
