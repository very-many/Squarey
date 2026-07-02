using UnityEngine;

public class HeartOfTheGiant : Upgrade
{
    public string upgradeImagePath => "Upgrades/Heart_Of_The_Giant";

    public string upgradeTitle => "Heart of the Giant";

    public string upgradeDescription => "Increases your health by " + _healthIncrease + "% and decreases your movement speed by 1";

    public int probabilityWeight => 10;

    private float _healthIncrease = 20f + Random.Range(1, 6);

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
        float newHealthMult = stats.GetHealthModifier() * (1 + (_healthIncrease / 100));
        stats.SetHealthModifier(newHealthMult);

        stats.SetMovementSpeed(stats.GetMovementSpeed() - 1);
    }
}
