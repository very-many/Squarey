using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class MultiStaffObject : MonoBehaviour {

    public GameObject player;
    public bool castBlocked = false;
    public DirectStaff directionalInfo;
    public Spellcasting spellcasting;

    public float MagicPower = 100;
    public float Recovery = 1f;
    public float ProjectileSize = 1f;
    public float ProjectileSpeed = 1f;

    public SingleStaff Staff_1;
    public SingleStaff Staff_2;
    public SingleStaff Staff_3;

    public List<Spell> Spellstorage;

    //TODO needs a connection to the player and to use the players Update Function
    public void Update()
    {
        Staff_1.FrameTicUpdate();
        Staff_2.FrameTicUpdate();
        Staff_3.FrameTicUpdate();
    }

    public void Awake()
    {
        player = gameObject;
        directionalInfo = GetComponentInChildren<DirectStaff>();
        directionalInfo.SetPlayer(player);

        spellcasting = GetComponent<Spellcasting>();

        Staff_1 = new SingleStaff(this, new List<Spell> { new Firebolt() });
        Staff_2 = new SingleStaff(this, null);
        Staff_3 = new SingleStaff(this, null);
        Spellstorage = new List<Spell>();
    }


    public void OnCast_1(InputAction.CallbackContext context)
    {
        if (castBlocked) { return; }
        castBlocked = true;
        Staff_1.CastSpells(this, context, directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle);
    }

    public void OnCast_2(InputAction.CallbackContext context)
    {
        if (castBlocked) { return; }
        castBlocked = true;
        Staff_2.CastSpells(this, context, directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle);
    }

    public void OnCast_3(InputAction.CallbackContext context)
    {
        if (castBlocked) { return; }
        castBlocked = true;
        Staff_3.CastSpells(this, context, directionalInfo.castDirection, directionalInfo.castPosition, directionalInfo.castAngle);
    }

    public void FinishCast()
    {
        castBlocked = false;
    }
}
