using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class Firebolt : Spell
{
    public float fireBoltRecoveryTime = 2;

    public float fireBoltCastTime = 0.2f;

    float Spell.spellRecoveryTime => fireBoltRecoveryTime;  
    float Spell.spellCastTime => fireBoltCastTime;
    string Spell.spellImagePath => "Spells/Firebolt";
    int Spell.probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        List<BulletType> bulletTypes = new List<BulletType> { Bullet.BulletType.Normal };
        float bulletDamage = 1f * multiStaff.bulletDamageMult * multiStaff.MagicPower;
        float bulletHealth = 0.1f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float bulletSize = 0.12f * multiStaff.ProjectileSize;
        float bulletSpeed = multiStaff.ProjectileSpeed;

        DirectStaff directionalInfo = multiStaff.directionalInfo;

        BulletStats bulletStats = new BulletStats(bulletTypes, bulletDamage, bulletHealth, bulletSize, bulletSpeed, Color.orange, multiStaff.player);

        multiStaff.spellcasting.CastBullet(directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle, bulletStats);
    }
}
