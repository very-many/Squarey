using UnityEngine;

public class Jump : Spell
{
    public string spellTitle => "Jump";

    public float spellRecoveryTime => 1.0f;

    public float spellCastTime => 0.4f;

    public string spellImagePath => "Spells/Jump";

    public int probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        multiStaff.spellcasting.Jump();
    }
}
