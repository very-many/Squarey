using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class StaffOfPower : Upgrade
{
    public string upgradeImagePath => "Upgrades/Staff_Of_Power";

    public string upgradeTitle => "Staff Of Power";

    public string upgradeDescription => "Increases your magic power by " + _magicPowerIncrease + " and increases your bullet damage by 10%";

    public int probabilityWeight => 20;

    private int _magicPowerIncrease = 30 + Random.Range(1, 16);

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
        stats.SetBulletDamageMult(stats.GetBulletDamageMult() * 1.10f);
    }
}
