using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class AcidSpray : Spell
{
    public string spellTitle => "Acid Spray";

    public float spellRecoveryTime => 3f;

    public float spellCastTime => 0.3f;

    public string spellImagePath => "Spells/AcidSpray";

    public int probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        List<BulletType> bulletTypes = new List<BulletType> { Bullet.BulletType.Physics };
        float bulletDamage = 0.65f * multiStaff.bulletDamageMult * multiStaff.MagicPower;
        float bulletHealth = 0.3f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float bulletSize = 0.06f * multiStaff.ProjectileSize;
        float bulletSpeed = 0.8f * multiStaff.ProjectileSpeed;

        BulletStats bulletStats = new BulletStats(bulletTypes, bulletDamage, bulletHealth, bulletSize, bulletSpeed, Color.greenYellow, multiStaff.player);

        CastAtAngle(0, multiStaff, singleStaff, bulletStats.Clone());
        CastAtAngle(5, multiStaff, singleStaff, bulletStats.Clone());
        CastAtAngle(-5, multiStaff, singleStaff, bulletStats.Clone());
        CastAtAngle(10, multiStaff, singleStaff, bulletStats.Clone());
        CastAtAngle(-10, multiStaff, singleStaff, bulletStats.Clone());

    }

    private void CastAtAngle(float angle, MultiStaffObject multiStaff, SingleStaff singleStaff, BulletStats bulletStats)
    {
        bulletStats.bulletSpeed += Random.Range(-0.1f, 0.1f);
        bulletStats.bulletSize += Random.Range(-0.02f, 0.02f);
        angle += Random.Range(-2f, 2f);

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
