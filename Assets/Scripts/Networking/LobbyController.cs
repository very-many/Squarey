using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    //UI Elements
    public TextMeshProUGUI LobbyNameText;

    //Player Data
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    //Other Data
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;

    //Ready
    public Button StartGameButton;
    public TextMeshProUGUI ReadyButtonText;

    //Manager
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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ReadyPlayer()
    {
        Debug.Log("Ready Player");
        LocalPlayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (LocalPlayerController.Ready)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;

        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (player.Ready)
            {
                AllReady = true;
            } 
            else 
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if(LocalPlayerController.PlayerID == 1)
            {
                StartGameButton.interactable = true;
            }

        }
        else
        {
            StartGameButton.interactable = false;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        GameOrchestrator.Instance?.OnPlayersChanged();  // Notify the GameOrchestrator that the player list has changed

        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }
        if (PlayerListItems.Count < Manager.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }
        if (PlayerListItems.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }
        if (PlayerListItems.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    private void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.SetPlayerValues();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    private void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.SetPlayerValues();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    private void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.SetPlayerValues();
                    if(player == LocalPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    private void RemovePlayerItem()
    {
        List<PlayerListItem> PlayerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem PlayerListItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == PlayerListItem.ConnectionID))
            {
                PlayerListItemToRemove.Add(PlayerListItem);
            }
        }
        if (PlayerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItemToRemove in PlayerListItemToRemove)
            {
                if (playerListItemToRemove != null)
                {
                    GameObject ObjectToRemove = playerListItemToRemove.gameObject;
                    PlayerListItems.Remove(playerListItemToRemove);
                    Destroy(ObjectToRemove);
                    ObjectToRemove = null;
                }
            }
        }
    }

    public void StartGame()
    {
        LocalPlayerController.CanStartGame();
    }
}
