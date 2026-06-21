using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public float bulletDamageMult = 1f;
    public float bulletHealthMult = 1f;
    public float ProjectileSize = 1f;
    public float ProjectileSpeed = 1f;

    public SingleStaff Staff_1;
    public SingleStaff Staff_2;
    public SingleStaff Staff_3;


    public void Update()
    {
        if (!isOwned) return;

        Staff_1.FrameTicUpdate();
        Staff_2.FrameTicUpdate();
        Staff_3.FrameTicUpdate();
    }

    public void Awake()
    {
        Staff_1 = new SingleStaff(this, new List<Spell> { new Firebolt() });
        Staff_2 = new SingleStaff(this, null);
        Staff_3 = new SingleStaff(this, new List<Spell> { new Jump() });
    }

    public void Start()
    {
        player = gameObject;
        directionalInfo = GetComponentInChildren<DirectStaff>();
        directionalInfo.SetPlayer(player);

        if (!isOwned) return;

        spellcasting = GetComponent<Spellcasting>();
    }

    public void UpdateSpells(List<Spell> staff1Spells, List<Spell> staff2Spells, List<Spell> staff3Spells)
    {
        Staff_1.UpdateSpells(staff1Spells);
        Staff_2.UpdateSpells(staff2Spells);
        Staff_3.UpdateSpells(staff3Spells);
    }


    public void OnCast_1(InputAction.CallbackContext context)
    {
        if (!context.started ||!isOwned) { return; }
        this.StartCoroutine(CastSequence(Staff_1, context));

    }

    public void OnCast_2(InputAction.CallbackContext context)
    {
        if (!context.started || !isOwned) { return; }
        this.StartCoroutine(CastSequence(Staff_2, context));
    }

    public void OnCast_3(InputAction.CallbackContext context)
    {
        if (!context.started || !isOwned) { return; }
        this.StartCoroutine(CastSequence(Staff_3, context));
    }

    private IEnumerator CastSequence(SingleStaff singleStaff, InputAction.CallbackContext context)
    {
        while (castBlocked && !context.canceled)
        {
            yield return null;
        }
            if (castBlocked) { yield break; }
        castBlocked = true;
        singleStaff.CastSpells(this, context);
        yield return null;
    }

    public void FinishCast()
    {
        castBlocked = false;
    }
}
