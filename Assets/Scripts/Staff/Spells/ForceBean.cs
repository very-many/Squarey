using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class ForceBean : Spell
{
    public string spellTitle => "Force Bean";

    public float spellRecoveryTime => 3f;

    public float spellCastTime => 0.2f;

    public string spellImagePath => "Spells/ForceBean";

    public int probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        List<BulletType> bulletTypes = new List<BulletType> { Bullet.BulletType.Physics, BulletType.Trail, BulletType.BounceOnWall, BulletType.DamageScaleWithSpeed};
        float bulletDamage = 0.8f * multiStaff.bulletDamageMult * multiStaff.MagicPower * multiStaff.ProjectileSpeed;
        float bulletHealth = 1.4f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float bulletSize = 0.15f * multiStaff.ProjectileSize;
        float bulletSpeed = 1.4f * multiStaff.ProjectileSpeed;

        DirectStaff directionalInfo = multiStaff.directionalInfo;

        BulletStats bulletStats = new BulletStats(bulletTypes, bulletDamage, bulletHealth, bulletSize, bulletSpeed, Color.darkRed, multiStaff.player);

        bulletStats.trailLength = 0.3f;
        bulletStats.bounciness = 1.2f;
        bulletStats.bounces = 10;
        bulletStats.timeToLive = 10f;

        multiStaff.spellcasting.CastBullet(directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle, bulletStats);
    }
}
