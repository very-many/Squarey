using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuCaller : NetworkBehaviour
{
    public UpgradeUI UpgradeUI;

    public StaffDragAndDrop StaffManager;

    public PlayerMainCoordinator coordinator;

    private void Start()
    {
        if (!isOwned) return;

        this.UpgradeUI = GameObject.FindGameObjectWithTag("UpgradePicker").GetComponent<UpgradeUI>();
        this.StaffManager = GameObject.FindGameObjectWithTag("StaffInventory").GetComponent<StaffDragAndDrop>();
        coordinator = GetComponent<PlayerMainCoordinator>();

        UpgradeUI.playerMainCoordinator = coordinator;

        StaffManager.playerMainCoordinator = coordinator;

        StaffManager.staffMulti = coordinator.GetMultiStaffObject();
    }

    public void OnChooseUpgrade(InputAction.CallbackContext context)
    {
        if (!context.performed || !isOwned) return;
        
        UpgradeUI.OpenUI(this);
    }

    public void OpenDragAndDrop()
    {
        StaffManager.OpenUI(this);
    }
}
