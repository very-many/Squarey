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


    [Header("State")]
    public GameState CurrentGameState { get; private set; } = GameState.Initial;
    public PlayerObjectController LastWinner { get; private set; }


    [Header("Scenes")]
    [SerializeField] private List<UnityEngine.Object> GameScenes;
    [SerializeField] private UnityEngine.Object UpgradeScene;

    [Header("Settings")]
    [SerializeField] private float switchDelay = 3f;


    public readonly SyncList<PlayerObjectController> readyPlayers = new();

    private CustomNetworkManager manager;
    private bool isSwitchingScene;

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
        if (CurrentGameState == GameState.Upgrade && UpgradeScene != null && scene.name == UpgradeScene.name)
        {
            SpawnPlayersUpgrade();
        }
        else if (CurrentGameState == GameState.Game && GameScenes != null && GameScenes.Any(gameScene => gameScene != null && gameScene.name == scene.name))
        {
            SpawnPlayersGame();
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
                    LastWinner = Players.Except(readyPlayers).FirstOrDefault();
                    Debug.Log("Game over! Last winner: " + (LastWinner != null ? LastWinner.name : "None") + ". Switching to upgrade state...");
                    EnterUpgradeState();
                }
                break;
            case GameState.Upgrade:
                Debug.Log("Is everyone ready? " + IsEveryoneReady() + ", isSwitchingScene: " + isSwitchingScene);
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
        Debug.Log("Checking if game is over: atLeastOneDead (" + atLeastOneDead + ") and lessThanOneAlive (" + lessThanOneAlive + "). -> " + (atLeastOneDead && lessThanOneAlive));
        return atLeastOneDead && lessThanOneAlive; //filter out players that left the game
    }

    bool IsEveryoneReady()
    {
        return readyPlayers.Count == Players.Count;
    }

    void OnReadyPlayersChanged(SyncList<PlayerObjectController>.Operation operation, int index, PlayerObjectController oldPlayer, PlayerObjectController newPlayer)
    {
        Debug.Log("Ready Players changed via " + operation + ": " + readyPlayers.Count + " ready player(s)! There are " + (Players.Count - readyPlayers.Count) + " left.");
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

        StartCoroutine(SwitchSceneAfterDelay(GameState.Upgrade, UpgradeScene.name, SpawnPlayersUpgrade));
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

        string randomScene = GameScenes[UnityEngine.Random.Range(0, GameScenes.Count)].name;
        StartCoroutine(SwitchSceneAfterDelay(GameState.Game, randomScene, SpawnPlayersGame));
    }

    private IEnumerator SwitchSceneAfterDelay(GameState nextState, string sceneName, Action preparePlayers)
    {
        isSwitchingScene = true;

        yield return new WaitForSeconds(switchDelay);

        CurrentGameState = nextState;
        readyPlayers.Clear();
        preparePlayers?.Invoke();

        if (Manager != null)
        {
            Manager.ServerChangeScene(sceneName);
        }

        isSwitchingScene = false;
    }

    public void SpawnPlayersGame()
    {
        foreach (var player in Players)
        {
            player.gameObject.SetActive(true);
            player.GetComponent<PlayerCosmeticController>().PlayerCosmeticsSetup();
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.GetComponent<PlayerInput>().enabled = true;
            player.GetComponent<PlayerMenuCaller>().playerUI.StartUI();
            player.GetComponent<PlayerMovementController>().RequestTeleport = true;
        }
    }

    public void SpawnPlayersUpgrade()
    {
        Debug.Log("Spawning players for upgrade state. Players count: " + Players.Count);
        foreach (var player in Players)
        {
            player.gameObject.SetActive(true);

            var visualRoot = player.transform.childCount > 0 ? player.transform.GetChild(0).gameObject : null;
            if (visualRoot != null)
            {
                visualRoot.SetActive(false);
            }

            player.GetComponent<PlayerInput>().enabled = false;
        }
    }
}
