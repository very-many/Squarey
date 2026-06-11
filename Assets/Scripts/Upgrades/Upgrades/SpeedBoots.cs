using Unity.VisualScripting;
using UnityEngine;

public class SpeedBoots : Upgrade
{
    public string upgradeImagePath => "Upgrades/Wizards_Boots";

    public string upgradeDescription => "Increases your movement speed by " + _speedIncrease + "% of its base value";

    public int probabilityWeight => 10;

    private int _speedIncrease = 20 + Random.Range(1, 12);

    public void ApplyUpgrade(PlayerMainCoordinator stats)
    {
        ApplySpeed(stats);
    }

    public void ReApplyUpgradeStats(PlayerMainCoordinator stats)
    {
        ApplySpeed(stats);
    }

    private void ApplySpeed(PlayerMainCoordinator stats)
    {
        float newSpeed = stats.GetMovementSpeed() + _speedIncrease/100f * stats.GetBaseMovementSpeed();
        stats.SetMovementSpeed(newSpeed);
    }
}
