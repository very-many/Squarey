
using UnityEngine;

public interface Spell
{
    float spellRecoveryTime { get; }
    float spellCastTime { get; }

    string spellImagePath { get; }

    void CastSpell(MultiStaffObject staff, Vector3 targetPosition, Quaternion targetRotation);
}
