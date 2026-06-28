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
    private static readonly int EndHash = Animator.StringToHash("End");
    private static readonly int StartHash = Animator.StringToHash("Start");

    public enum GameState
    {
        Initial,
        Game,
        Upgrade
    }

    public static GameOrchestrator Instance { get; private set; }

    [SerializeField] private TMPro.TextMeshProUGUI timer;
    [SerializeField] private Animator transitionAnimation;

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
    [SerializeField] private float transitionDelay = 0.5f;


    public readonly SyncList<PlayerObjectController> readyPlayers = new();

    private CustomNetworkManager manager;
    private bool isSwitchingScene;
    private bool startTransitionAfterSceneLoad;
    private bool startPostSceneCountdownAfterLoad;

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
        if (!startTransitionAfterSceneLoad)
        {
            if (startPostSceneCountdownAfterLoad && isServer)
            {
                startPostSceneCountdownAfterLoad = false;
                StartCoroutine(PostSceneCountdown());
            }

            SyncCountdownLabel();
            return;
        }

        startTransitionAfterSceneLoad = false;

        if (transitionAnimation != null)
        {
            transitionAnimation.SetTrigger(StartHash);
        }

        SyncCountdownLabel();

        if (startPostSceneCountdownAfterLoad && isServer)
        {
            startPostSceneCountdownAfterLoad = false;
            StartCoroutine(PostSceneCountdown());
        }
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
        isSwitchingScene = true;

        currentCountdown = 0;
        RpcPlayTransitionEnd();
        yield return new WaitForSeconds(transitionDelay);

        currentGameState = nextState;
        readyPlayers.Clear();

        ResetUpgradeReadyStatus();

        preparePlayers?.Invoke();

        if (Manager != null)
        {
            RpcPlayTransitionStart();
            startTransitionAfterSceneLoad = true;
            startPostSceneCountdownAfterLoad = nextState == GameState.Game;
            Manager.ServerChangeScene(sceneName);
        }

        isSwitchingScene = false;
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
        if (transitionAnimation != null)
        {
            transitionAnimation.SetTrigger(EndHash);
        }
    }
    [ClientRpc]
    private void RpcPlayTransitionStart()
    {
        startTransitionAfterSceneLoad = true;
    }

    [ClientRpc]
    private void RpcSpawnPlayersGame()
    {
        foreach (var player in Players)
        {
            player.gameObject.SetActive(true);
            player.GetComponent<PlayerCosmeticController>().PlayerCosmeticsSetup();
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.GetComponent<PlayerInput>().enabled = true;
            player.GetComponent<MultiStaffObject>().castLocked = true;

            // Only the local player's controller should start UI and request teleport
            var playerNetworkIdentity = player.GetComponent<NetworkIdentity>();
            if (playerNetworkIdentity != null && playerNetworkIdentity.isLocalPlayer)
            {
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

            player.GetComponent<PlayerInput>().enabled = false;

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
        //TODO
        throw new NotImplementedException();
    }
}
