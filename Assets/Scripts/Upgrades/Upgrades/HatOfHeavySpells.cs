using UnityEngine;

public class HatOfHeavySpells : Upgrade
{
    public string upgradeImagePath => "Upgrades/Hat_Of_Heavy_Spells";

    public string upgradeTitle => "Hat of heavy Spells";

    public string upgradeDescription => "Increases damage and size of bullets by " + _spellWeightIncrease + "%, reduces bullet speed by 10%";

    public int probabilityWeight => 10;

    private float _spellWeightIncrease = 20f + Random.Range(1, 6);

    public void ApplyUpgrade(PlayerMainCoordinator stats)
    {
        ApplySpellWeight(stats);
    }

    public void ReApplyUpgradeStats(PlayerMainCoordinator stats)
    {
        ApplySpellWeight(stats);
    }

    private void ApplySpellWeight(PlayerMainCoordinator stats)
    {
        float newDamageMult = stats.GetBulletDamageMult() * (1 + (_spellWeightIncrease/100));
        stats.SetBulletDamageMult(newDamageMult);

        float newProjectileSize = stats.GetStaffProjectileSize() * (1 + (_spellWeightIncrease / 100));
        stats.SetStaffProjectileSize(newProjectileSize);

        float newSpellSpeed = stats.GetStaffProjectileSpeed() * 0.9f;
        stats.SetStaffProjectileSpeed(newSpellSpeed);
    }
}
