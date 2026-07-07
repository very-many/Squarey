using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    [SerializeField] private GameOrchestrator gameOrchestratorPrefab;
    //public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
    public List<PlayerObjectController> GamePlayers = new List<PlayerObjectController>();

    private bool isManualDisconnectInProgress;
    private bool handleUnexpectedDisconnect;
    private string unexpectedDisconnectOfflineScene;

    [Header("Pooling")]
    [SerializeField] private ObjectPool bulletPool;


    public override void Start()
    {
        base.Start();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        if (bulletPool != null)
            bulletPool.PrewarmServer();

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            TrySpawnGameOrchestrator();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (bulletPool != null)
            bulletPool.RegisterClientHandlers();
    }

    public void PrepareForManualDisconnect()
    {
        isManualDisconnectInProgress = true;
        handleUnexpectedDisconnect = false;
        unexpectedDisconnectOfflineScene = "";
    }

    public override void OnClientDisconnect()
    {
        if (isManualDisconnectInProgress)
            return;

        unexpectedDisconnectOfflineScene = offlineScene;
        offlineScene = "";
        handleUnexpectedDisconnect = true;

        base.OnClientDisconnect();
    }

    public override void OnStopHost()
    {
        GamePlayers.Clear();
        base.OnStopHost();
    }

    public override void OnStopServer()
    {
        GamePlayers.Clear();
        base.OnStopServer();
    }

    public override void OnStopClient()
    {
        GamePlayers.Clear();
        base.OnStopClient();

        if (handleUnexpectedDisconnect)
        {
            handleUnexpectedDisconnect = false;
            Invoke(nameof(HandleUnexpectedDisconnectCleanup), 0f);
        }

        isManualDisconnectInProgress = false;
    }

    private void HandleUnexpectedDisconnectCleanup()
    {
        if (GameOrchestrator.Instance != null)
        {
            GameOrchestrator.Instance.HandleClientDisconnect(unexpectedDisconnectOfflineScene);
        }
        else
        {
            GetComponent<SteamLobby>()?.LeaveLobby();

            if (!string.IsNullOrWhiteSpace(unexpectedDisconnectOfflineScene))
            {
                SceneManager.LoadScene(unexpectedDisconnectOfflineScene);
                offlineScene = unexpectedDisconnectOfflineScene;
            }
        }

        unexpectedDisconnectOfflineScene = "";
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == "Lobby")
        {
            TrySpawnGameOrchestrator();
        }
    }

    private void TrySpawnGameOrchestrator()
    {
        if (SceneManager.GetActiveScene().name != "Lobby")
            return;

        if (GameOrchestrator.Instance != null)
            return;

        if (gameOrchestratorPrefab == null)
        {
            Debug.LogError("Cannot spawn GameOrchestrator: prefab reference is missing.");
            return;
        }

        if (!NetworkServer.active)
            return;

        GameOrchestrator orchestratorInstance = Instantiate(gameOrchestratorPrefab);
        NetworkServer.Spawn(orchestratorInstance.gameObject);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerID = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);
        
            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public void StartGame()
    {
        if (GameOrchestrator.Instance != null)
        {
            GameOrchestrator.Instance.NextGameState();
            return;
        }

        Debug.LogError($"Cannot start game: GameOrchestrator is not available.");
    }
}
