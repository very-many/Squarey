using System.Collections;
using UnityEngine;
using Mirror;

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

        elapsed = 0f;
        while (elapsed < timeoutSeconds)
        {
            if (NetworkClient.ready)
            {
                var playerCallers = FindObjectsOfType<PlayerMenuCaller>();
                bool foundOwned = false;
                foreach (var caller in playerCallers)
                {
                    if (caller.isOwned)
                    {
                        foundOwned = true;
                        break;
                    }
                }
                if (foundOwned) break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        var allCallers = FindObjectsOfType<PlayerMenuCaller>();
        foreach (var caller in allCallers)
        {
            if (!caller.isOwned) continue;
            caller.WireAndOpenUpgrade();
        }
    }
}
