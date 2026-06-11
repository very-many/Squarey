using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class StaffOfPower : Upgrade
{
    public string upgradeImagePath => "Upgrades/Staff_Of_Power";

    public string upgradeDescription => "Increases your magic power by " + _magicPowerIncrease + " and increases your bullet damage by 1%";

    public int probabilityWeight => 20;

    private int _magicPowerIncrease = 10 + Random.Range(1, 6);

    public void ApplyUpgrade(PlayerMainCoordinator stats)
    {
        ApplyPower(stats);
    }

    public void ReApplyUpgradeStats(PlayerMainCoordinator stats)
    {
        ApplyPower(stats);
    }

    private void ApplyPower(PlayerMainCoordinator stats)
    {
        float newPower = stats.GetStaffPower() + _magicPowerIncrease;
        stats.SetStaffPower(newPower);
        stats.SetBulletDamageMult(stats.GetBulletDamageMult() * 1.01f);
    }
}
