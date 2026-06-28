using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    //public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
    public List<PlayerObjectController> GamePlayers = new List<PlayerObjectController>();

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
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (bulletPool != null)
            bulletPool.RegisterClientHandlers();
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

    public void StartGame(string SceneName)
    {
        if (GameOrchestrator.Instance != null)
        {
            GameOrchestrator.Instance.NextGameState();
            return;
        }

        Debug.LogError($"Cannot start game for scene '{SceneName}': GameOrchestrator is not available.");
    }
}
