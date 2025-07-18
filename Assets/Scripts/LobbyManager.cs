using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private TextMeshProUGUI playersConnectedText;
    [SerializeField] private SceneRef gameScene; // Asigná la escena Game en el Inspector
    private bool _loadingScene = false;

    private NetworkRunner _runner;

    private void Start()
    {
        _runner = FindObjectOfType<NetworkRunner>();
        _runner.AddCallbacks(this);
        UpdateConnectedPlayersText();
    }

    private void UpdateConnectedPlayersText()
    {
        if (_runner != null && playersConnectedText != null)
        {
            int playerCount = _runner.ActivePlayers.Count();
            playersConnectedText.text = $"Jugadores conectados: {playerCount}/2";
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_runner == null || _loadingScene) return;

        int playerCount = _runner.ActivePlayers.Count();

        if (playerCount >= 2)
        {
            _loadingScene = true;
            LoadGameScene();
        }
    }

private async void LoadGameScene()
{
    _runner.RemoveCallbacks(this);

    Debug.Log("Cargando escena de juego...");

    var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
    if (sceneManager == null)
    {
        sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
    }

    var result = await _runner.StartGame(new StartGameArgs
    {
        GameMode = GameMode.Shared,
        SessionName = "Sala_01",
        SceneManager = sceneManager
    });

    if (!result.Ok)
    {
        await _runner.SceneManager.LoadScene(gameScene, new NetworkLoadSceneParameters());
        Debug.LogError($"Error al cargar escena de juego: {result.ShutdownReason}");
        _loadingScene = false;
    }
}


    // Este callback se llama cuando alguien se conecta
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        UpdateConnectedPlayersText();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        UpdateConnectedPlayersText();
    }

    // El resto de los callbacks no los necesitamos ahora:
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)    {   }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)    {           }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)    {          }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)    {          }
}
