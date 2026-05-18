using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiStaffObject : MonoBehaviour {

    public GameObject player;
    public Quaternion castAngle;
    public Vector2 castPosition;
    public Vector2 targetPosition;
    public bool isCasting = false;

    public float MagicPower = 100;
    public float Recovery = 1f;
    public float ProjectileSize = 1f;
    public float ProjectileSpeed = 1f;

    public SingleStaff Staff_1;
    public SingleStaff Staff_2;
    public SingleStaff Staff_3;

    public List<Spell> Spellstorage;

    public MultiStaffObject()
    {
        Staff_1 = new SingleStaff(this, null);
        Staff_2 = new SingleStaff(this, null);
        Staff_3 = new SingleStaff(this, null);
        Spellstorage = new List<Spell>();
    }

    //TODO needs a connection to the player and to use the players Update Function
    public void Update()
    {
        Staff_1.FrameTicUpdate();
        Staff_2.FrameTicUpdate();
        Staff_3.FrameTicUpdate();
    }

    public void OnCast_1(InputAction.CallbackContext context)
    {
        if (isCasting) { return; }
        isCasting = true;
        Staff_1.CastSpells(this, context, Input.mousePosition, castAngle);
    }

    public void OnCast_2(InputAction.CallbackContext context)
    {
        if (isCasting) { return; }
        isCasting = true;
        Staff_2.CastSpells(this, context, Input.mousePosition, castAngle);
    }

    public void OnCast_3(InputAction.CallbackContext context)
    {
        if (isCasting) { return; }
        isCasting = true;
        Staff_3.CastSpells(this, context, Input.mousePosition, castAngle);
    }

    public void FinishCast()
    {
        isCasting = false;
    }
}
