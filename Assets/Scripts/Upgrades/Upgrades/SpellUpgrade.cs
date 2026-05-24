using UnityEngine;

public class SpellUpgrade : Upgrade
{
    public string upgradeImagePath => _upgradeSpell.spellImagePath;

    public string upgradeDescription => "Adds the " + _upgradeSpell.GetType() + " Spell to your Spellpool and gives " + _recoveryBonus + " Recovery stat so cooldowns aren't as long.";

    public int probabilityWeight => 100;

    private Spell _upgradeSpell = new SpellLibrary().RandomSpell();

    private int _recoveryBonus = Random.Range(1, 4);

    public void ApplyUpgrade(PlayerMainCoordinator stats)
    {
        stats.SpellPool.Add(_upgradeSpell);
        stats.SetStaffRecovery(stats.GetStaffRecovery() + _recoveryBonus);
    }
    public void ReApplyUpgradeStats(PlayerMainCoordinator stats)
    {
        stats.SetStaffRecovery(stats.GetStaffRecovery() + _recoveryBonus);
    }
}
