using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class MultiStaffObject : NetworkBehaviour {

    public GameObject player;
    public bool castBlocked = false;
    public DirectStaff directionalInfo;
    public Spellcasting spellcasting;
    public PlayerUI playerUI;

    public float MagicPower = 100;
    public float Recovery = 100f;
    public float ProjectileSize = 1f;
    public float ProjectileSpeed = 1f;

    public SingleStaff Staff_1;
    public SingleStaff Staff_2;
    public SingleStaff Staff_3;

    //TODO needs a connection to the player and to use the players Update Function
    public void Update()
    {
        if (!isOwned) return;
        Staff_1.FrameTicUpdate();
        Staff_2.FrameTicUpdate();
        Staff_3.FrameTicUpdate();
    }

    public void Start()
    {
        player = gameObject;
        directionalInfo = GetComponentInChildren<DirectStaff>();
        directionalInfo.SetPlayer(player);

        if (!isOwned) return;

        spellcasting = GetComponent<Spellcasting>();

        Staff_1 = new SingleStaff(this, new List<Spell> { new Firebolt() });
        Staff_2 = new SingleStaff(this, null);
        Staff_3 = new SingleStaff(this, null);

        playerUI = GetComponent<PlayerUI>();
        playerUI.staffMulti = this;
    }

    public void UpdateSpells(List<Spell> staff1Spells, List<Spell> staff2Spells, List<Spell> staff3Spells)
    {
        Staff_1.UpdateSpells(staff1Spells);
        Staff_2.UpdateSpells(staff2Spells);
        Staff_3.UpdateSpells(staff3Spells);

        playerUI.InsertSpells();
    }


    public void OnCast_1(InputAction.CallbackContext context)
    {
        if (!context.started || !isOwned || castBlocked) { return; }    
        castBlocked = true;
        Staff_1.CastSpells(this, context);
    }

    public void OnCast_2(InputAction.CallbackContext context)
    {
        if (!context.started || !isOwned || castBlocked) { return; }
        castBlocked = true;
        Staff_2.CastSpells(this, context);
    }

    public void OnCast_3(InputAction.CallbackContext context)
    {
        if (!context.started || !isOwned || castBlocked) { return; }
        castBlocked = true;
        Staff_3.CastSpells(this, context);
    }

    public void FinishCast()
    {
        castBlocked = false;
    }
}
