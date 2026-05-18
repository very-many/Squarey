using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SingleStaff
{
    public MultiStaffObject ParentStaffMulti;
    public List<Spell> SpellList;
    public float spellCoolDownTimer = 0;

    public SingleStaff(MultiStaffObject parentStaffMulti, List<Spell>? spellList)
    {
        ParentStaffMulti = parentStaffMulti;
        SpellList = spellList;
        if (SpellList == null)
        {
            SpellList = new List<Spell>();
        }
    }

    public void CastSpells(MultiStaffObject staffMulti, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (spellCoolDownTimer > 0)
        {
            return;
        }

        float cooldownTime = 0;

        foreach (Spell spell in SpellList)
        {
            spell.CastSpell(staffMulti, targetPosition, targetRotation);
            cooldownTime = cooldownTime + (spell.spellRecoveryTime/ParentStaffMulti.Recovery);

            if (false || cooldownTime > 10) { break; } //TODO if the staff isn't being cast anymore; break
        }
        spellCoolDownTimer = cooldownTime;
    }

    public void AddSpell(Spell spell)
    {
        SpellList.Add((Spell)System.Activator.CreateInstance(spell.GetType()));
    }

    public void FrameTicUpdate() { 
        if (spellCoolDownTimer > 0) {
            spellCoolDownTimer -= Time.deltaTime;
        }
    }
}