using System.Collections;
using UnityEngine;
using Mirror;

public class UpgradeController : MonoBehaviour
{
    private const float timeoutSeconds = 5f;

    private UpgradeUI upgradeUI;
    private StaffDragAndDrop staffUI;
    private bool readyCached;
    private int currentReady;
    private int currentPlayers;


    private void Awake ()
    {
        upgradeUI = FindFirstObjectByType<UpgradeUI>();
        staffUI = FindFirstObjectByType<StaffDragAndDrop>();

        upgradeUI.UpgradeUIReady += OnUIReady;
        staffUI.StaffUIReady += OnUIReady;
        GameOrchestrator.Instance.PlayerCountChanged += OnPlayerCountChanged;
    }

    private void OnDestroy()
    {
        if (upgradeUI != null)
            upgradeUI.UpgradeUIReady -= OnUIReady;

        if (staffUI != null)
            staffUI.StaffUIReady -= OnUIReady;
        if(GameOrchestrator.Instance != null)
            GameOrchestrator.Instance.PlayerCountChanged -= OnPlayerCountChanged;
    }

    private void OnEnable()
    {
        readyCached = false;    //reset readyCached when the object is enabled
    }

    private void Start()
    {
        StartCoroutine(WaitAndWireUI());
    }

    private void OnUIReady()    //OnUIReady -> RefreshPlayerCount -> OnPlayerCountChanged -> UpdateReadyPlayersText
    {
        if(!readyCached)
        {
            readyCached = true;
            GameOrchestrator.Instance?.RefreshPlayerCount();
        }
        

        UpdateReadyPlayersText();

    }

    private void OnPlayerCountChanged(int readyCount, int playerCount) //OnPlayerCountChanged -> UpdateReadyPlayersText 
    {
        currentReady = readyCount;
        currentPlayers = playerCount;

        UpdateReadyPlayersText();
    }


    public void UpdateReadyPlayersText()
    {
        string text;
        bool localReady = GameOrchestrator.Instance?.RefreshLocalPlayerReadyStatus() ?? false;


        if (currentPlayers > 1 && currentReady == currentPlayers - 1)
        {
            text = localReady
                ? "Waiting for one player..."
                : "Everyone is waiting for you!";
        }
        else
        {
            text = $"{currentReady}/{currentPlayers} Players ready!";
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