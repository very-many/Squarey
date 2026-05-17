using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiStaffObject
{
    public int MagicPower = 100;
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
    public void FrameTicUpdate()
    {
        Staff_1.FrameTicUpdate();
        Staff_2.FrameTicUpdate();
        Staff_2.FrameTicUpdate();
    }
}
