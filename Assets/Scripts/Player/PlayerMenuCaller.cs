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

    public bool canOpenMenu = true;

    private void Start()
    {
        if (!isOwned) return;
        coordinator = GetComponent<PlayerMainCoordinator>();

        this.playerUI = GetComponent<PlayerUI>();

        //if not steam game
        Debug.Log("Game Orchestrator Instance: " + GameOrchestrator.Instance);
        if (GameOrchestrator.Instance == null)
            playerUI.StartUI();
    }

    public void OnChooseUpgrade(InputAction.CallbackContext context)
    {
        if (!canOpenMenu) return;

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

        //if not steam game
        if (GameOrchestrator.Instance == null)
            playerUI.StartUI();

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
            //OPTION 1: send to staffDragAndDrop
            //staffManager.OpenUI(this);

            //Option2: send to waiting screen
            staffManager.OpenWaitingUI(this);
            player.SetUpgradeReady();
        }
        else
        {
            //usually open the upgrade UI for non-winners
            upgradeUI.OpenUI(this);
        }
    }
}
