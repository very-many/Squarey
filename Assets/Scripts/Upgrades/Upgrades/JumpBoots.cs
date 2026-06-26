using UnityEngine;

public class JumpBoots : Upgrade
{
    public string upgradeImagePath => "Upgrades/Jump_Boots";

    public string upgradeTitle => "Jump Boots";

    public string upgradeDescription => "Increases your jump force by " + _jumpIncrease.ToString("F2");

    public int probabilityWeight => 10;

    private float _jumpIncrease = ((float)(15 + Random.Range(1, 7))) / 20;

    public void ApplyUpgrade(PlayerMainCoordinator stats)
    {
        ApplyJump(stats);
    }

    public void ReApplyUpgradeStats(PlayerMainCoordinator stats)
    {
        ApplyJump(stats);
    }

    private void ApplyJump(PlayerMainCoordinator stats)
    {
        float newJumpForce = stats.GetJumpForce() + _jumpIncrease;
        stats.SetJumpForce(newJumpForce);
    }
}
