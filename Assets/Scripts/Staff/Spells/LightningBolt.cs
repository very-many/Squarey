using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class LightningBolt : Spell
{
    public string spellTitle => "Lightning Bolt";

    public float spellRecoveryTime => 2.5f;

    public float spellCastTime => 0.1f;

    public string spellImagePath => "Spells/LightningBolt";

    public int probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        List<BulletType> bulletTypes = new List<BulletType> { Bullet.BulletType.Normal, BulletType.BounceOnWall, BulletType.Trail};
        float bulletDamage = 0.7f * multiStaff.bulletDamageMult * multiStaff.MagicPower;
        float bulletHealth = 1f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float bulletSize = 0.04f * multiStaff.ProjectileSize;
        float bulletSpeed = 2f * multiStaff.ProjectileSpeed;

        DirectStaff directionalInfo = multiStaff.directionalInfo;

        BulletStats bulletStats = new BulletStats(bulletTypes, bulletDamage, bulletHealth, bulletSize, bulletSpeed, Color.purple, multiStaff.player);

        multiStaff.spellcasting.CastBullet(directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle, bulletStats);
    }
}
