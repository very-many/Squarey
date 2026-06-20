using System.Collections;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    private const float timeoutSeconds = 5f;

    void Start()
    {
        StartCoroutine(WaitAndWireUI());
    }

    private IEnumerator WaitAndWireUI()
    {
        float elapsed = 0f;
        GameObject upgradePicker = null;
        GameObject staffInventory = null;

        while (elapsed < timeoutSeconds)
        {
            upgradePicker = GameObject.FindGameObjectWithTag("UpgradePicker");
            staffInventory = GameObject.FindGameObjectWithTag("StaffInventory");
            if (upgradePicker != null && staffInventory != null)
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        var playerCallers = FindObjectsOfType<PlayerMenuCaller>();
        foreach (var caller in playerCallers)
        {
            if (!caller.isOwned) continue;
            caller.WireAndOpenUpgrade();
        }
    }
}
