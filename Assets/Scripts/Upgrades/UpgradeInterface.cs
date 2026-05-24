using UnityEngine;
using UnityEngine.Rendering;

public interface Upgrade
{
    string upgradeImagePath { get; }

    string upgradeDescription { get; }

    int probabilityWeight { get; }

    void ApplyUpgrade(PlayerMainCoordinator stats);

    void ReApplyUpgradeStats(PlayerMainCoordinator stats);
}
