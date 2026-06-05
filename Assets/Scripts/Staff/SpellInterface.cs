
using UnityEngine;

public interface Spell
{
    float spellRecoveryTime { get; }
    float spellCastTime { get; }
    string spellImagePath { get; }
    int probabilityWeight { get; }

    void CastSpell(MultiStaffObject staff, Vector2 castDirection, Vector2 castPosition, Quaternion targetRotation);
}
