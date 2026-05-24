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

    private bool isCasting = false;

    public SingleStaff(MultiStaffObject parentStaffMulti, List<Spell>? spellList)
    {
        ParentStaffMulti = parentStaffMulti;
        SpellList = spellList;
        if (SpellList == null)
        {
            SpellList = new List<Spell>();
        }
    }

    public void CastSpells(MultiStaffObject staffMulti, InputAction.CallbackContext context, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (spellCoolDownTimer > 0)
        {
            return;
        }

        float cooldownTime = 0;

        foreach (Spell spell in SpellList)
        {
            CastTime(spell.spellCastTime);

            spell.CastSpell(staffMulti, targetPosition, targetRotation);
            cooldownTime = cooldownTime + (spell.spellRecoveryTime/ParentStaffMulti.Recovery);

            while (isCasting) { var v = Task.Yield(); } // Wait until the cast time is over before proceeding to the next spell

            if (false || cooldownTime > 10) { break; }
            if (context.canceled) { break; }
        }
        spellCoolDownTimer = cooldownTime;
        staffMulti.FinishCast();
    }

    IEnumerator CastTime(float duration)
    {
        isCasting = true;
        yield return new WaitForSeconds(duration);
        isCasting = false;
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