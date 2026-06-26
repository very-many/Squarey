using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class Thunderwave : Spell
{
    public string spellTitle => "Thunderwave";

    public float spellRecoveryTime => 2;

    public float spellCastTime => 0.25f;

    public string spellImagePath => "Spells/Thunderwave";

    public int probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        List<BulletType> bulletTypes = new List<BulletType> { BulletType.Normal, BulletType.Trail, BulletType.KnockBackFromPlayer };
        float bulletDamage = 0.4f * multiStaff.bulletDamageMult * multiStaff.ProjectileSize * multiStaff.MagicPower;
        float bulletHealth = 2.5f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float bulletSize = 0.05f * multiStaff.ProjectileSize;
        float bulletSpeed = 3f * multiStaff.ProjectileSpeed;

        DirectStaff directionalInfo = multiStaff.directionalInfo;

        BulletStats bulletStats = new BulletStats(bulletTypes, bulletDamage, bulletHealth, bulletSize, bulletSpeed, Color.purple, multiStaff.player);

        bulletStats.trailLength = 1.5f;
        bulletStats.sprite = SpriteLibrary.SpriteType.Capsule;
        bulletStats.timeToLive = 0.07f;
        bulletStats.knockbackForce = 30f;

        CastAtAngle(-24, multiStaff, singleStaff, bulletStats);
        CastAtAngle(-12, multiStaff, singleStaff, bulletStats);
        CastAtAngle(0, multiStaff, singleStaff, bulletStats);
        CastAtAngle(12, multiStaff, singleStaff, bulletStats);
        CastAtAngle(24, multiStaff, singleStaff, bulletStats);
    }

    private void CastAtAngle(float angle, MultiStaffObject multiStaff, SingleStaff singleStaff, BulletStats bulletStats)
    {
        DirectStaff directionalInfo = multiStaff.directionalInfo;
        RotateAngle(angle, directionalInfo.castAngle, directionalInfo.castDirection, out Quaternion newAngle, out Vector2 newDirection);
        multiStaff.spellcasting.CastBullet(newDirection, directionalInfo.castPosition, newAngle, bulletStats);
    }

    private void RotateAngle(float angle, Quaternion orientationToChange, Vector2 directionToChange, out Quaternion newOrientation, out Vector2 newDirection)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        newOrientation = rotation * orientationToChange;

        newDirection = rotation * directionToChange;
    }
}
