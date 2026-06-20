using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuCaller : NetworkBehaviour
{
    public UpgradeUI upgradeUI;

    public StaffDragAndDrop staffManager;

    public PlayerUI playerUI;

    public PlayerMainCoordinator coordinator;

    private void Start()
    {
        if (!isOwned) return;
        coordinator = GetComponent<PlayerMainCoordinator>();

        this.playerUI = GetComponent<PlayerUI>();
        //playerUI.StartUI();
    }

    public void OnChooseUpgrade(InputAction.CallbackContext context)
    {
        if (!context.performed || !isOwned) return;
        if (upgradeUI == null)
        {
            if (!WireExternalUi())
            {
                Debug.LogError("Failed to wire external UI components.");
                return;
            }
        }
        playerUI.StopUI();
        upgradeUI.OpenUI(this);
    }

    public void OpenDragAndDrop()
    {
        if (staffManager == null)
        {
            if (!WireExternalUi())
            {
                Debug.LogError("Failed to wire external UI components.");
                return;
            }
        }
        staffManager.OpenUI(this);
    }

    public void CloseDragAndDrop()
    {
        if (staffManager == null || upgradeUI == null)
        {
            if (!WireExternalUi())
            {
                Debug.LogError("Failed to wire external UI components.");
                return;
            }
        }
        staffManager.CloseUI();
        //playerUI.StartUI();
    }

    public void CloseHud()
    {
        playerUI.StopUI();
    }

    public void OpenHud()
    {
        playerUI.StartUI();
    }

    public bool WireExternalUi()
    {
        this.upgradeUI = GameObject.FindGameObjectWithTag("UpgradePicker")?.GetComponent<UpgradeUI>();
        this.staffManager = GameObject.FindGameObjectWithTag("StaffInventory")?.GetComponent<StaffDragAndDrop>();

        Debug.Log("Found Upgrade UI: " + (upgradeUI != null).ToString());
        Debug.Log("Found Staff Manager: " + (staffManager != null).ToString());
        if (upgradeUI == null || staffManager == null || coordinator == null) return false;

        upgradeUI.playerMainCoordinator = coordinator;
        staffManager.playerMainCoordinator = coordinator;
        staffManager.staffMulti = coordinator.GetMultiStaffObject();
        return true;
    }

    public void WireAndOpenUpgrade()
    {
        if (!isOwned) return;
        if (!WireExternalUi())
        {
            Debug.LogError("Failed to wire external UI components on scene load.");
            return;
        }
        if (playerUI != null)
            playerUI.StopUI();
        PlayerObjectController player = GetComponent<PlayerObjectController>();
        if (GameOrchestrator.Instance.LastWinner == player)
        {
            // Auto-ready the winner without showing upgrade UI
            player.SetUpgradeReady();
        }
        else
        {
            upgradeUI.OpenUI(this);
        }
    }
}
