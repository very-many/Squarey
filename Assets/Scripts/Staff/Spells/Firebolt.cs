using System.Collections.Generic;
using UnityEngine;
using static Bullet;
using SmallHedge.SoundManager;

public class Firebolt : Spell
{

    public string spellTitle => "Firebolt";

    float Spell.spellRecoveryTime => 2f;
    float Spell.spellCastTime => 0.2f;
    string Spell.spellImagePath => "Spells/Firebolt";
    int Spell.probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        if (multiStaff == null || multiStaff.directionalInfo == null || multiStaff.spellcasting == null)
        {
            Debug.LogError("Firebolt: Missing required components (multiStaff " + multiStaff + ", directionalInfo " + multiStaff.directionalInfo + ", or spellcasting " + multiStaff.spellcasting + ")");
            return;
        }

        // Play sound effect
        Debug.Log("Firebolt: Playing sound effect for Firebolt spell.");
        SoundManager.PlaySound(SoundType.Spell_Firebolt);

        List<BulletType> bulletTypes = new List<BulletType> { Bullet.BulletType.Normal, BulletType.Trail };
        float bulletDamage = 1f * multiStaff.bulletDamageMult * multiStaff.MagicPower;
        float bulletHealth = 1f * multiStaff.bulletHealthMult * multiStaff.MagicPower;
        float bulletSize = 0.05f * multiStaff.ProjectileSize;
        float bulletSpeed = multiStaff.ProjectileSpeed;

        DirectStaff directionalInfo = multiStaff.directionalInfo;

        BulletStats bulletStats = new BulletStats(bulletTypes, bulletDamage, bulletHealth, bulletSize, bulletSpeed, Color.orangeRed, multiStaff.player);

        bulletStats.trailLength = 0.04f;

        multiStaff.spellcasting.CastBullet(directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle, bulletStats);
    }
}
