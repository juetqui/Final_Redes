using Fusion;
using UnityEngine;
using System.Collections;
using Fusion.Sockets;
using System;

[RequireComponent(typeof(NetworkRunner))]
public class PlayerSpawner : SimulationBehaviour, INetworkRunnerCallbacks
{
    [Header("Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private NetworkRunner _runner;

    private void Awake()
    {
        _runner = GetComponent<NetworkRunner>();
    }

    private IEnumerator Start()
    {
        // Esperar a que el runner esté listo
        while (_runner == null || !_runner.IsRunning)
            yield return null;

        // Registrarse para recibir callbacks
        _runner.AddCallbacks(this);
        Debug.Log("[PlayerSpawner] Callbacks registered.");

        // Spawnear a los jugadores que ya estén conectados
        foreach (var p in _runner.ActivePlayers)
            SpawnIfNeeded(p);
    }

    /* -------------------- Fusion callbacks -------------------- */

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[PlayerSpawner] Player {player} joined.");
        SpawnIfNeeded(player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[PlayerSpawner] Player {player} left.");
    }

    /* -------------------- Spawning -------------------- */

    private void SpawnIfNeeded(PlayerRef player)
    {
        if (_runner.TryGetPlayerObject(player, out _)) return;

        int index = player.RawEncoded % spawnPoints.Length;
        Transform spawn = spawnPoints[index];

        _runner.Spawn(playerPrefab, spawn.position, spawn.rotation, player);
        Debug.Log($"[PlayerSpawner] Spawned player {player} at point {index}");
    }

    /* -------------------- Empty callbacks (required) -------------------- */

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
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)    {    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)    {           }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)    {        }
}   