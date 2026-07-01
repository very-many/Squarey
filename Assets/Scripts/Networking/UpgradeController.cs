using System.Collections;
using UnityEngine;
using Mirror;

public class UpgradeController : MonoBehaviour
{
    private const float timeoutSeconds = 5f;

    private UpgradeUI upgradeUI;
    private StaffDragAndDrop staffUI;

    private void Awake()
    {
        upgradeUI = FindFirstObjectByType<UpgradeUI>();
        staffUI = FindFirstObjectByType<StaffDragAndDrop>();

        if (upgradeUI != null)
            upgradeUI.UpgradeUIReady += OnUIReady;

        if (staffUI != null)
            staffUI.StaffUIReady += OnUIReady;
    }

    private void OnDestroy()
    {
        if (upgradeUI != null)
            upgradeUI.UpgradeUIReady -= OnUIReady;

        if (staffUI != null)
            staffUI.StaffUIReady -= OnUIReady;
    }

    private void Start()
    {
        StartCoroutine(WaitAndWireUI());
    }

    private void OnEnable()
    {
        if (GameOrchestrator.Instance != null)
            GameOrchestrator.Instance.ReadyPlayersChanged += UpdateReadyPlayersText;
    }

    private void OnDisable()
    {
        if (GameOrchestrator.Instance != null)
            GameOrchestrator.Instance.ReadyPlayersChanged -= UpdateReadyPlayersText;
    }

    private void OnUIReady()
    {
        if (GameOrchestrator.Instance == null)
            return;

        UpdateReadyPlayersText(
            GameOrchestrator.Instance.readyPlayers.Count,
            GameOrchestrator.Instance.PlayerCount,
            GameOrchestrator.Instance.readyPlayers.Contains(GameOrchestrator.Instance.LocalPlayer));
    }

    public void UpdateReadyPlayersText(int readyCount, int playerCount, bool localPlayerReady)
    {
        string text;
    
        if (playerCount > 1 && readyCount == playerCount - 1)
        {
            text = localPlayerReady
                ? "Waiting for one player..."
                : "Everyone is waiting for you!";
        }
        else
        {
            text = $"{readyCount}/{playerCount} Players ready!";
        }
    
        upgradeUI?.SetReadyPlayersText(text);
        staffUI?.SetReadyPlayersText(text);
    }

    private IEnumerator WaitAndWireUI()
    {
        float elapsed = 0f;

        while (elapsed < timeoutSeconds)
        {
            if (GameObject.FindGameObjectWithTag("UpgradePicker") != null &&
                GameObject.FindGameObjectWithTag("StaffInventory") != null)
            {
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < timeoutSeconds)
        {
            if (NetworkClient.ready)
            {
                foreach (var caller in FindObjectsOfType<PlayerMenuCaller>())
                {
                    if (caller.isOwned)
                    {
                        caller.WireAndOpenUpgrade();
                        yield break;
                    }
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}