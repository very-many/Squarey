using UnityEngine;

public class Firebolt : Spell
{
    public float fireBoltRecoveryTime = 2;

    public float fireBoltCastTime = 0.5f;

    float Spell.spellRecoveryTime => fireBoltRecoveryTime;  
    float Spell.spellCastTime => fireBoltCastTime;
    string Spell.spellImagePath => "Spells/Firebolt";
    int Spell.probabilityWeight => 10;

    public void CastSpell(MultiStaffObject statStaffMulti, Vector3 targetPosition, Quaternion targetRotation)
    {
        throw new System.NotImplementedException();
    }
}
