using UnityEngine;

public class SpellSniper : Upgrade
{
    public string upgradeImagePath => "Upgrades/Spell_Sniper";

    public string upgradeTitle => "Spell Sniper";

    public string upgradeDescription => "Increases your magic power by " + _magicPowerIncrease + "% and increases your bullet speed by " + _projectileSpeedIncrease + "%";

    public int probabilityWeight => 10;

    private int _magicPowerIncrease = 10 + Random.Range(1, 6);

    private float _projectileSpeedIncrease = 20f + Random.Range(1, 6);

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

        float newProjectileSpeedMult = stats.GetStaffProjectileSpeed() * (1 + (_projectileSpeedIncrease / 100));
        stats.SetStaffProjectileSpeed(newProjectileSpeedMult);
    }
}
