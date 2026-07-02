using Unity.VisualScripting;
using UnityEngine;

public class SpeedBoots : Upgrade
{
    public string upgradeImagePath => "Upgrades/Wizards_Boots";

    public string upgradeTitle => "Speed Boots";

    public string upgradeDescription => "Increases your movement speed by " + _speedIncrease.ToString("F1") + " and your jump force by " + _jumpIncrease.ToString("F2");

    public int probabilityWeight => 10;

    private float _speedIncrease = ((float)(25 + Random.Range(1, 11))) /10;

    private float _jumpIncrease = ((float)(25 + Random.Range(1, 10))) / 30;

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
        float newSpeed = stats.GetMovementSpeed() + _speedIncrease;
        stats.SetMovementSpeed(newSpeed);
        float newJumpForce = stats.GetJumpForce() + _jumpIncrease;
        stats.SetJumpForce(newJumpForce);
    }
}
