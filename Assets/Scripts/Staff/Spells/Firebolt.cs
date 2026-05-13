using UnityEngine;

public class Firebolt : Spell
{
    public float fireBoltRecoveryTime = 2;

    public float fireBoltCastTime = 0.5f;

    float Spell.spellRecoveryTime => fireBoltRecoveryTime;  
    float Spell.spellCastTime => fireBoltCastTime;
    string Spell.spellImagePath => "Assets/ImageAssetImports(Josa)/Spells/Firebolt.png";

    public void CastSpell(MultiStaffObject staff, Vector3 targetPosition, Quaternion targetRotation)
    {
        throw new System.NotImplementedException();
    }
}
