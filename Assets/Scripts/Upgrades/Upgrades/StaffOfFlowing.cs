using UnityEngine;

public class StaffOfFlowing : Upgrade
{
    public string upgradeImagePath => "Upgrades/Staff_Of_Flowing";

    public string upgradeTitle => "Staff Of Flowing";

    public string upgradeDescription => "Increases Recovery by " + _recoveryModifier + "% and decreases spell damage by 25%";

    public int probabilityWeight => 10;

    private float _recoveryModifier = 50f + Random.Range(1, 6);

    public void ApplyUpgrade(PlayerMainCoordinator stats)
    {
        ApplyFlowing(stats);
    }

    public void ReApplyUpgradeStats(PlayerMainCoordinator stats)
    {
        ApplyFlowing(stats);
    }

    private void ApplyFlowing(PlayerMainCoordinator stats) {
        float newRecoveryMod = stats.GetRecoveryModifier() * (1 + _recoveryModifier/100);
        stats.SetRecoveryModifier(newRecoveryMod);

        float newBulletDamageMult = stats.GetBulletDamageMult() * 0.75f;
        stats.SetBulletDamageMult(newBulletDamageMult);
    }
}
