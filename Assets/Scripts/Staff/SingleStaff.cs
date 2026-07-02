using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class SingleStaff
{
    public MultiStaffObject ParentStaffMulti;
    public List<Spell> SpellList;
    public float spellCoolDownTimer = 0;
    private float castTimer = 0;

    public SingleStaff(MultiStaffObject parentStaffMulti, List<Spell> spellList)
    {
        ParentStaffMulti = parentStaffMulti;
        SpellList = spellList;
        if (SpellList == null)
        {
            SpellList = new List<Spell>();
        }
    }

    public void UpdateSpells(List<Spell> newSpellList)
    {
        SpellList = newSpellList;
    }

    public void CastSpells(MultiStaffObject staffMulti, InputAction.CallbackContext context)
    {
        if (spellCoolDownTimer > 0)
        {
            staffMulti.FinishCast();
            return;
        }

        staffMulti.StartCoroutine(CastSequence(staffMulti, context));
    }

    IEnumerator WaitForCast()
    {
        while (castTimer > 0)
            yield return null;
    }

    private IEnumerator CastSequence(MultiStaffObject staffMulti, InputAction.CallbackContext context)
    {
        float cooldownTime = 0;
        foreach (Spell spell in SpellList)
        {
            castTimer = spell.spellCastTime;

            spell.CastSpell(staffMulti, this);
            cooldownTime = cooldownTime + GetCooldownWithModifiers(staffMulti, spell);

            yield return staffMulti.StartCoroutine(WaitForCast());

            if (!context.action.IsPressed()){ break; }
        }

        spellCoolDownTimer = cooldownTime;

        staffMulti.FinishCast();
    }

    private float GetCooldownWithModifiers(MultiStaffObject staffMulti, Spell currentSpell)
    {
        float baseCooldown = currentSpell.spellRecoveryTime / staffMulti.Recovery * 100;

        int spellIndex = SpellList.IndexOf(currentSpell);

        float amountModifier = (float)(Math.Pow(0.8, spellIndex));

        float uniqueModifier = 1f;

        for (int i = spellIndex - 1; i > -1; i--)
        {
            if (currentSpell.spellTitle != SpellList[i].spellTitle)
            {
                uniqueModifier *= 0.8f;
            } else { break; }
        }

        return baseCooldown*amountModifier*uniqueModifier;
    }


    public void AddSpell(Spell spell)
    {
        SpellList.Add((Spell)System.Activator.CreateInstance(spell.GetType()));
    }

    public void FrameTicUpdate() { 
        if (spellCoolDownTimer > 0) {
            spellCoolDownTimer -= Time.deltaTime;
        }
        if (castTimer > 0)
        {
            castTimer -= Time.deltaTime;
        }
    }
}