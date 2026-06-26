using System.Collections.Generic;
using UnityEngine;
using static Bullet;
using SmallHedge.SoundManager;

public class AcidSplash : Spell
{
    public string spellTitle => "Acid Splash";

    public float spellRecoveryTime => 4f;

    public float spellCastTime => 0.3f;

    public string spellImagePath => "Spells/AcidSplash";

    public int probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        // Play sound effect
        SoundManager.PlaySound(SoundType.Spell_AcidSplash);

        List<BulletType> bulletTypes = new List<BulletType> { Bullet.BulletType.Physics, BulletType.Trail, BulletType.Split, BulletType.BounceOnWall };
        float bulletDamage = 0.5f * multiStaff.bulletDamageMult * multiStaff.MagicPower;
        float bulletHealth = 0.7f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float bulletSize = 0.2f * multiStaff.ProjectileSize;
        float bulletSpeed = 0.8f * multiStaff.ProjectileSpeed;

        DirectStaff directionalInfo = multiStaff.directionalInfo;

        BulletStats bulletStats = new BulletStats(bulletTypes, bulletDamage, bulletHealth, bulletSize, bulletSpeed, Color.greenYellow, multiStaff.player);
        bulletStats.timeToLive = 2.5f;
        bulletStats.trailLength = 0.04f;
        bulletStats.bounces = 1;
        bulletStats.sprite = SpriteLibrary.SpriteType.Circle;


        List<BulletType> miniBulletTypes = new List<BulletType> { Bullet.BulletType.Physics, BulletType.BounceOnWall};
        float miniBulletDamage = 0.25f * multiStaff.bulletDamageMult * multiStaff.MagicPower;
        float miniBulletHealth = 0.1f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float miniBulletSize = 0.05f * multiStaff.ProjectileSize;
        float miniBulletSpeed = 0.7f * multiStaff.ProjectileSpeed;

        BulletStats miniBulletStats = new BulletStats(miniBulletTypes, miniBulletDamage, miniBulletHealth, miniBulletSize, miniBulletSpeed, Color.greenYellow, multiStaff.player);
        miniBulletStats.timeToLive = 0.7f;
        miniBulletStats.timeToEscape = 0.0f;
        miniBulletStats.bounces = 2;

        
        for (int i = 0; i < 10; i++)
        {
            miniBulletStats.splitAngleOffset += 36;
            bulletStats.splitBullets.Add(miniBulletStats.Clone());
        }

        multiStaff.spellcasting.CastBullet(directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle, bulletStats);
    }
}
