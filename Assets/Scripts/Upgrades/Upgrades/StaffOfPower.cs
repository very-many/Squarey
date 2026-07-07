using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class StaffOfPower : Upgrade
{
    public string upgradeImagePath => "Upgrades/Staff_Of_Power";

    public string upgradeTitle => "Staff Of Power";

    public string upgradeDescription => "Increases your magic power by " + _magicPowerIncrease + " and increases your bullet damage by 5%";

    public int probabilityWeight => 10;

    private int _magicPowerIncrease = 20 + Random.Range(1, 13);

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
        stats.SetBulletDamageMult(stats.GetBulletDamageMult() * 1.05f);
    }
}
