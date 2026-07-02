using UnityEngine;

public class ArmourOfHealth :Upgrade
{
    public string upgradeImagePath => "Upgrades/Armour_Of_Health";

    public string upgradeTitle => "Armour of Health";

    public string upgradeDescription => "Increases your health by " + _healthIncrease + " and increases your bullet health by 15%";

    public int probabilityWeight => 20;

    private int _healthIncrease = 120 + Random.Range(1, 50);

    public void ApplyUpgrade(PlayerMainCoordinator stats)
    {
        ApplyHealth(stats);
    }

    public void ReApplyUpgradeStats(PlayerMainCoordinator stats)
    {
        ApplyHealth(stats);
    }

    private void ApplyHealth(PlayerMainCoordinator stats)
    {
        int newMaxHealth = stats.GetMaxHealth() + _healthIncrease;
        stats.SetMaxHealth(newMaxHealth);
        stats.SetBulletHealthMult(stats.GetBulletHealthMult() * 1.15f);
    }
}
