using System.Collections.Generic;
using UnityEngine;

public class Firebolt : Spell
{
    public float fireBoltRecoveryTime = 2;

    public float fireBoltCastTime = 0.5f;

    float Spell.spellRecoveryTime => fireBoltRecoveryTime;  
    float Spell.spellCastTime => fireBoltCastTime;
    string Spell.spellImagePath => "Spells/Firebolt";
    int Spell.probabilityWeight => 10;

    public void CastSpell(MultiStaffObject statStaffMulti, Vector2 castDirection, Vector2 castPosition, Quaternion targetRotation)
    {
        float damage = 1f * statStaffMulti.MagicPower;
        float health = 0.1f * statStaffMulti.MagicPower;
        float size = 0.5f * statStaffMulti.ProjectileSize;

        statStaffMulti.spellcasting.castBullet(castDirection, castPosition, targetRotation, damage, health, size, new List<Bullet.BulletType> { Bullet.BulletType.Normal });
    }
}
