using Mirror;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameOrchestrator : NetworkBehaviour
{
    public enum GameState
    {
        Initial,
        Game,
        Upgrade
    }

    public static GameOrchestrator Instance { get; private set; }

    [SerializeField] private TMPro.TextMeshProUGUI timer;

    [Header("State")]
    [SyncVar]
    private GameState currentGameState = GameState.Initial;

    [SyncVar]
    private PlayerObjectController lastWinner;

    [SyncVar(hook = nameof(OnCountdownChanged))]
    private int currentCountdown = 0;

    public GameState CurrentGameState => currentGameState;
    public PlayerObjectController LastWinner => lastWinner;
    public int CurrentCountdown => currentCountdown;

    [Header("Scenes")]
    [SerializeField] private List<string> GameScenes;
    [SerializeField] private string UpgradeScene;

    [Header("Settings")]
    [SerializeField] private const int countdownStart = 3;


    public readonly SyncList<PlayerObjectController> readyPlayers = new();

    private CustomNetworkManager manager;
    private bool isSwitchingScene;
    private bool startPostSceneCountdownAfterLoad;

    private PlayerObjectController localPlayer;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
                return manager;

            return manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    private List<PlayerObjectController> Players
    {
        get
        {
            if (Manager == null)
                return new List<PlayerObjectController>();
            return Manager.GamePlayers;
        }
    }
    public int PlayerCount => Players.Count;             //make player count available before Scene- change
    public event Action<int, int, bool> ReadyPlayersChanged;  //! Subscribed by UpgradeController

    public PlayerObjectController LocalPlayer
    {
        get
        {
            if (Players == null || Players.Count == 0)
                return null;
            if (localPlayer == null)
            {
                localPlayer = Players.FirstOrDefault(p => p.GetComponent<NetworkIdentity>().isLocalPlayer);
            }
            return localPlayer;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            readyPlayers.Callback += OnReadyPlayersChanged;
        }
        else
        {
            Debug.Log("GameOrchestrator already exists, destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (startPostSceneCountdownAfterLoad && isServer)
        {
            startPostSceneCountdownAfterLoad = false;
            StartCoroutine(PostSceneCountdown());
        }

        SyncCountdownLabel();
    }

    private void OnCountdownChanged(int oldValue, int newValue)
    {
        SyncCountdownLabel();
    }

    private void SyncCountdownLabel()
    {
        if (timer != null)
        {
            timer.text = currentCountdown > 0 ? currentCountdown.ToString() : "";
        }
    }

    public void NextGameState()
    {
        ShouldSwitchGameState();
    }

    void ShouldSwitchGameState()
    {
        if (!isServer)
            return;

        switch (CurrentGameState)
        {
            case GameState.Initial:
                EnterGameState();
                break;
            case GameState.Game:
                if (IsGameOver() && !isSwitchingScene)
                {
                    lastWinner = Players.Except(readyPlayers).FirstOrDefault();
                    EnterUpgradeState();
                }
                break;
            case GameState.Upgrade:
                if (IsEveryoneReady() && !isSwitchingScene)
                {
                    EnterGameState();
                }
                break;
            default:
                Debug.LogError("You fucked up your gameStates! Shame on you: " + CurrentGameState);
                break;
        }
    }

    bool IsGameOver()
    {
        bool atLeastOneDead = readyPlayers.Count > 0;
        bool lessThanOneAlive = Players.Count - readyPlayers.Count <= 1;
        return atLeastOneDead && lessThanOneAlive; //filter out players that left the game
    }

    bool IsEveryoneReady()
    {
        return readyPlayers.Count >= Players.Count;
    }

    void OnReadyPlayersChanged(SyncList<PlayerObjectController>.Operation operation, int index, PlayerObjectController oldPlayer, PlayerObjectController newPlayer)
    {
        Debug.Log(readyPlayers.Count + " ready player(s)! There are " + (Players.Count - readyPlayers.Count) + " left.");
        if (!isServer)
            return;

        ShouldSwitchGameState();
        
        ReadyPlayersChanged?.Invoke(
            readyPlayers.Count,
            Players.Count,
            readyPlayers.Contains(LocalPlayer));
        }

    void EnterUpgradeState()
    {
        if (isSwitchingScene)
            return;

        if (Manager == null || UpgradeScene == null)
        {
            Debug.LogError("Cannot enter upgrade state: missing network manager or upgrade scene.");
            return;
        }

        StartCoroutine(SwitchSceneAfterDelay(GameState.Upgrade, UpgradeScene, () => RpcSpawnPlayersUpgrade()));
    }

    void EnterGameState()
    {
        if (isSwitchingScene)
            return;

        if (Manager == null || GameScenes == null || GameScenes.Count == 0)
        {
            Debug.LogError("Cannot enter game state: missing network manager or game scenes.");
            return;
        }

        string randomScene = GameScenes[UnityEngine.Random.Range(0, GameScenes.Count)];
        StartCoroutine(SwitchSceneAfterDelay(GameState.Game, randomScene, () => RpcSpawnPlayersGame()));
    }

    
    private IEnumerator SwitchSceneAfterDelay(GameState nextState, string sceneName, Action preparePlayers)
    {
        if (isSwitchingScene)
            yield break;

        isSwitchingScene = true;

        currentCountdown = 0;

        RpcPlayTransitionEnd();
        yield return new WaitForSeconds(SceneTransitionManager.Instance != null
            ? SceneTransitionManager.Instance.TransitionDelay
            : 0.5f);

        Action finishSceneSwitch = () =>
        {
            currentGameState = nextState;
            readyPlayers.Clear();

            ResetUpgradeReadyStatus();

            preparePlayers?.Invoke();

            if (Manager != null)
            {
                startPostSceneCountdownAfterLoad = nextState == GameState.Game;
                Manager.ServerChangeScene(sceneName);
            }

            isSwitchingScene = false;
        };

        finishSceneSwitch();
    }

    private IEnumerator PostSceneCountdown()
    {
        for (int countdown = countdownStart; countdown > 0; countdown--)
        {
            currentCountdown = countdown;
            yield return new WaitForSeconds(1f);
        }

        currentCountdown = 0;
        RpcEnablePlayerCasting();
    }

    private void ResetUpgradeReadyStatus()
    {
        foreach (var player in Players)
        {
            if (player != null)
            {
                player.UpgradeReady = false;
            }
        }
    }

    [ClientRpc]
    private void RpcPlayTransitionEnd()
    {
        SceneTransitionManager.Instance?.PlayTransitionEnd();
    }

    [ClientRpc]
    private void RpcSpawnPlayersGame()
    {
        foreach (var player in Players)
        {
            player.gameObject.SetActive(true);
            player.GetComponent<PlayerCosmeticController>().PlayerCosmeticsSetup();

            var visualRoot = player.transform.childCount > 0 ? player.transform.GetChild(0).gameObject : null;
            if (visualRoot != null)
            {
                visualRoot.SetActive(true);
            }

            // Only the local player's controller should start UI and request teleport
            if (player == LocalPlayer)
            {
                EnableLocalPlayerInput(true);
                player.GetComponent<MultiStaffObject>().castLocked = true;
                player.GetComponent<PlayerMenuCaller>().playerUI.StartUI();
                player.GetComponent<PlayerMovementController>().RequestTeleport = true;
            }
        }
    }

    [ClientRpc]
    private void RpcSpawnPlayersUpgrade()
    {
        foreach (var player in Players)
        {
            player.gameObject.SetActive(true);

            var visualRoot = player.transform.childCount > 0 ? player.transform.GetChild(0).gameObject : null;
            if (visualRoot != null)
            {
                visualRoot.SetActive(false);
            }

            if (player == LocalPlayer) DisableLocalPlayerInput();

            //ready players for game state -- in upgrade state
            player.GetComponent<MultiStaffObject>().castBlocked = false;
            player.GetComponent<Health>().ResetHealth();
            player.GetComponent<PlayerMovementController>().canMove = true;
        }
    }

    [ClientRpc]
    private void RpcEnablePlayerCasting()
    {
        foreach (var player in Players)
        {
            if (player != null)
            {
                player.GetComponent<MultiStaffObject>().castLocked = false;
            }
        }
    }

    public void AddPlayerReady(PlayerObjectController player)
    {
        if (!isServer || player == null)
            return;
        if (!readyPlayers.Contains(player))
        {
            readyPlayers.Add(player);
        }
    }

    public void RemovePlayerReady(PlayerObjectController player)
    {
        if (!isServer || player == null)
            return;
        if (readyPlayers.Contains(player))
        {
            readyPlayers.Remove(player);
        }
    }

    internal void LeaveGame()
    {
        string offlineScene = "";

        if (Manager != null)
        {
            offlineScene = Manager.offlineScene;
            Manager.offlineScene = string.Empty;

            if (NetworkServer.active && NetworkClient.isConnected)
            {
                Manager.StopHost();
                Debug.Log("Manager still alive? " + NetworkManager.singleton.isActiveAndEnabled, NetworkManager.singleton);
            }
            else if (NetworkClient.isConnected)
            {
                Manager.StopClient();
            }
            else if (NetworkServer.active)
            {
                Manager.StopServer();
            }
        }

        PauseMenu.Instance.DisableAllMenus();

        if (!string.IsNullOrWhiteSpace(offlineScene))
        {
            SceneManager.LoadScene(offlineScene);

            if (Manager != null)
            {
                Manager.offlineScene = offlineScene;
            }
        }

        Destroy(gameObject);

    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    internal void DisableLocalPlayerInput()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.GetComponent<PlayerInput>().enabled = false;
        }
        else
        {
            Debug.LogWarning("No local player found to disable input.");
        }
    }

    internal void EnableLocalPlayerInput(bool force = false)
    {
        EnablePlayerInput(LocalPlayer, force);
    }

    private void EnablePlayerInput(PlayerObjectController player, bool force = false)
    {
        Debug.Log("Enabling input for player: " + (player != null ? player.PlayerName : "null"));
        if (currentGameState != GameState.Game && !force)
        {
            Debug.LogWarning("Cannot enable local player input: not in Game state.");
            return;
        }
        if (player != null)
        {
            player.GetComponent<PlayerInput>().enabled = true;
        }
        else
        {
            Debug.LogWarning("No player found to enable input.");
        }
    }
}
