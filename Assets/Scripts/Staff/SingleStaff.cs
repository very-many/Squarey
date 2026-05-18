using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Threading.Tasks;

public class SingleStaff
{
    public MultiStaffObject ParentStaffMulti;
    public List<Spell> SpellList;
    public float spellCoolDownTimer = 0;
    public float spellCastTimer = 0;

    public SingleStaff(MultiStaffObject parentStaffMulti, List<Spell>? spellList)
    {
        ParentStaffMulti = parentStaffMulti;
        SpellList = spellList;
        if (SpellList == null)
        {
            SpellList = new List<Spell>();
        }
    }

    public async Task CastSpells(MultiStaffObject staffMulti, InputAction.CallbackContext context, Vector3 targetPosition, Quaternion targetRotation)
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

            while (spellCoolDownTimer > 0) { await Task.Yield(); }

            if (false || cooldownTime > 10) { break; } //TODO if the staff isn't being cast anymore; break
            if (context.canceled) { break; }
        }
        spellCoolDownTimer = cooldownTime;
        staffMulti.FinishCast();
    }

    public void AddSpell(Spell spell)
    {
        SpellList.Add((Spell)System.Activator.CreateInstance(spell.GetType()));
    }

    public void FrameTicUpdate() { 
        if (spellCoolDownTimer > 0) {
            spellCoolDownTimer -= Time.deltaTime;
        }
        if (spellCastTimer > 0) {
            spellCastTimer -= Time.deltaTime;
        }
    }
}