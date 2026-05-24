using System.Collections.Generic;
using UnityEngine;

public class UpgradeLibrary : MonoBehaviour
{
    public static UpgradeLibrary instance;

    private List<Upgrade> _allUpgradesList = new List<Upgrade> { new SpellUpgrade() };

    public Upgrade RandomUpgrade()
    {
        int randomNumber = Random.Range(0, GetTotalUpgradeProbabilityWeight());
        Upgrade selectedUpgrade = _allUpgradesList[GetUpgradeIndexFromRandomNumber(randomNumber)];
        return (Upgrade)System.Activator.CreateInstance(selectedUpgrade.GetType());
    }

    private int GetTotalUpgradeProbabilityWeight()
    {
        int totalWeight = 0;
        for (int i = 0; i < _allUpgradesList.Count; i++)
        {
            totalWeight = totalWeight + _allUpgradesList[i].probabilityWeight;

        }
        return totalWeight;
    }

    private int GetUpgradeIndexFromRandomNumber(int randomWeightResult)
    {
        for (int i = 0; i < _allUpgradesList.Count; i++)
        {
            randomWeightResult = randomWeightResult - _allUpgradesList[i].probabilityWeight;
            if (randomWeightResult <= 0)
            {
                return i;
            }

        }
        // this line should never be executed and is a failsafe
        return Random.RandomRange(0, _allUpgradesList.Count);
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
