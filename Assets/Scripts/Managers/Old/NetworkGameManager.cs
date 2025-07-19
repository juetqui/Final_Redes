using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static NetworkGameManager Instance;

    [Header("General")]
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private GameObject PlayerSpawnerPrefab;

    [Header("Scene References")]
    [SerializeField] private SceneRef mainMenuScene;
    [SerializeField] private SceneRef lobbyScene;
    [SerializeField] private SceneRef gameScene;

    private NetworkRunner _runner;
    private bool _isHost;
    private bool _initialized;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public async void StartSession(bool isHost)
    {
        if (_isHost == false)
            _isHost = isHost;

        if (_runner == null)
        {
            _runner = Instantiate(runnerPrefab);
            _runner.name = "NetworkRunner";
            DontDestroyOnLoad(_runner.gameObject);
        }

        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);

        var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
            sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "Sala_01",
            SceneManager = sceneManager
        });

        if (result.Ok)
        {
            await LoadSceneAsync(lobbyScene);
        }
        else
        {
            Debug.LogError($"Error al iniciar la sesión: {result.ShutdownReason}");
        }

    }

    public async Task LoadSceneAsync(SceneRef targetScene)
    {
        if (_runner == null || _runner.SceneManager == null)
        {
            Debug.LogError("Runner o SceneManager no están inicializados");
            return;
        }

        if (targetScene == lobbyScene)
        {
            await _runner.SceneManager.LoadScene(targetScene, new NetworkLoadSceneParameters());

            if (SceneManager.GetSceneByBuildIndex(mainMenuScene.AsIndex).isLoaded)
            {
                await _runner.SceneManager.UnloadScene(mainMenuScene);
            }
        }

        if (targetScene == gameScene)
        {
            await _runner.SceneManager.LoadScene(targetScene, new NetworkLoadSceneParameters());

            if (SceneManager.GetSceneByBuildIndex(lobbyScene.AsIndex).isLoaded)
            {
                await _runner.SceneManager.UnloadScene(lobbyScene);
            }
        }

        Debug.Log($"[NGM] LoadSceneAsync terminado. Escena activa: {SceneManager.GetActiveScene().name} (index={SceneManager.GetActiveScene().buildIndex})");

    }
    /* --------- Callback de Fusion --------- */
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Scene currentScene = SceneManager.GetSceneByBuildIndex(gameScene.AsIndex);

        if (currentScene.isLoaded)
        {
            Debug.Log($"[NGM] OnSceneLoadDone: escena {currentScene.name} cargada correctamente.");
            runner.Spawn(PlayerSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("[NGM] PlayerSpawner spawned.");
        }
        else
        {
            Debug.LogWarning($"[NGM] OnSceneLoadDone: escena esperada {gameScene} no está marcada como cargada todavía.");
        }
    }


    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
}
