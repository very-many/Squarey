using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class PlayerObjectController : NetworkBehaviour
{
    private const string PrefKeyCosmeticIndex = "CurrentCosmeticIndex";

    //Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    //Upgrade Phase
    [SyncVar(hook = nameof(OnUpgradeReadyChanged))] public bool UpgradeReady;

    //Cosmetics
    [SyncVar(hook = nameof(SendPlayerCosmetic))] public int PlayerCosmetic;
    [SerializeField] private TextMeshProUGUI PlayerNameText;
    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
                return manager;
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    private void PlayerReadyUpdate(bool OldValue, bool NewValue)
    {
        if (isServer)
        {
            this.Ready = NewValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    private void OnUpgradeReadyChanged(bool OldValue, bool NewValue)
    {
        if (isServer)
        {
            // Notify GameOrchestrator about upgrade ready status change
            if (GameOrchestrator.Instance != null)
            {
                if (NewValue)
                {
                    GameOrchestrator.Instance.AddPlayerReady(this);
                }
                else
                {
                    GameOrchestrator.Instance.RemovePlayerReady(this);
                }
            }
        }
    }

    [Command]
    public void CmdSetUpgradeReady()
    {
        if (isServer)
        {
            UpgradeReady = true;
        }
    }

    public void SetUpgradeReady()
    {
        if (isOwned)
        {
            CmdSetUpgradeReady();
        }
    }

    public void ChangeReady()
    {
        if (isOwned)
        {
            CmdSetPlayerReady();
        }
    }





    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();

        int cosmeticIndex = PlayerPrefs.GetInt(PrefKeyCosmeticIndex, 0);
        CmdUpdatePlayerCosmetic(cosmeticIndex);
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();    // + Notify the GameOrchestrator that the player list has changed
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this); //TODO; this line seems to be causing an error sometimes
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    private void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer)
        {
            this.PlayerName = NewValue;
        }
        if (isClient)
        {
            if (PlayerNameText != null)
            {
                PlayerNameText.text = NewValue;
            }
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame()
    {
        if (isOwned)
        {
            CmdCanStartGame();
        }
    }

    [Command]
    public void CmdCanStartGame()
    {
        manager.StartGame();
    }

    //Cosmetics

    [Command]
    public void CmdUpdatePlayerCosmetic(int NewValue)
    {
        SendPlayerCosmetic(this.PlayerCosmetic, NewValue);
    }

    public void SendPlayerCosmetic(int OldValue, int NewValue)
    {
        if (isServer)
        {
            this.PlayerCosmetic = NewValue;
        }
        if (isClient && (OldValue != NewValue))
        {
            UpdadeCosmetic(NewValue);
        }
    }

    void UpdadeCosmetic(int message)
    {
        this.PlayerCosmetic = message;
    }

}
