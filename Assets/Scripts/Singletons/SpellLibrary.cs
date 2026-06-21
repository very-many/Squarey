using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpellLibrary : MonoBehaviour
{
    public static SpellLibrary instance;

    private List<Spell> _allSpellList = new List<Spell> { 
        new Firebolt(), new Jump(), new AcidSpray(), 
        new LightningBolt(), new AcidSplash(), new Snowball(), 
        new ForceBean(), new Thunderwave(),
        };


    public Spell RandomSpell()
    {
        int randomNumber = Random.Range(0, GetTotalSpellProbabilityWeight());
        Spell selectedSpell = _allSpellList[GetSpellIndexFromRandomNumber(randomNumber)];
        return (Spell)System.Activator.CreateInstance(selectedSpell.GetType());
    }

    public int GetTotalSpellProbabilityWeight()
    {
        int totalWeight = 0;
        for (int i = 0; i < _allSpellList.Count; i++)
        {
            totalWeight = totalWeight + _allSpellList[i].probabilityWeight;

        }
        return totalWeight;
    }

    private int GetSpellIndexFromRandomNumber(int randomWeightResult)
    {
        for (int i = 0; i < _allSpellList.Count; i++)
        {
            randomWeightResult = randomWeightResult - _allSpellList[i].probabilityWeight;
            if (randomWeightResult <= 0)
            {
                return i;
            }

        }
        // this line should never be executed and is a failsafe
        return Random.RandomRange(0, _allSpellList.Count);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}