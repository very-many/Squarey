using UnityEngine;
using SmallHedge.SoundManager;

public class Jump : Spell
{
    public string spellTitle => "Jump";

    public float spellRecoveryTime => 0.75f;

    public float spellCastTime => 0.4f;

    public string spellImagePath => "Spells/Jump";

    public int probabilityWeight => 10;

    public void CastSpell(MultiStaffObject multiStaff, SingleStaff singleStaff)
    {
        // Play sound effect
        SoundManager.PlaySound(SoundType.Spell_Jump);

        multiStaff.spellcasting.Jump();

    }
}
