using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpellLibrary : MonoBehaviour
{
    public static SpellLibrary instance;

    public List<Spell> allSpellList = new List<Spell> { new Firebolt() };

    public Spell RandomSpell()
    {
        int randomIndex = Random.Range(0, allSpellList.Count);
        Spell selectedSpell = allSpellList[randomIndex];
        return (Spell)System.Activator.CreateInstance(selectedSpell.GetType());
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}