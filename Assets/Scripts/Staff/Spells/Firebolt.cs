using System.Collections.Generic;
using UnityEngine;

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
        float damage = 1f * multiStaff.MagicPower;
        float health = 0.1f * multiStaff.MagicPower;
        float size = 0.5f * multiStaff.ProjectileSize;

        DirectStaff directionalInfo = multiStaff.directionalInfo;

        multiStaff.spellcasting.CastBullet(directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle, damage, health, size, new List<Bullet.BulletType> { Bullet.BulletType.Normal });
    }
}
