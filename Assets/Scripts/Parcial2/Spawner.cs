using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Linq;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _gameManagerPrefab;
    [SerializeField] private Transform[] _spawnTransforms;

    private bool _initialized;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer && GameManager.Instance == null)
        {
            runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity);
        }

        var playersCount = runner.ActivePlayers.Count();

        if (_initialized && playersCount >= 2)
        {
            CreatePlayer(0, runner);
            return;
        }

        if (player == runner.LocalPlayer)
        {
            if (playersCount < 2)
            {
                _initialized = true;
            }
            else
            {
                CreatePlayer(playersCount - 1, runner);
            }
        }
    }

    void CreatePlayer(int spawnPointIndex, NetworkRunner runner)
    {
        _initialized = false;
        spawnPointIndex = spawnPointIndex % _spawnTransforms.Length;
        var newPosition = _spawnTransforms[spawnPointIndex].position;
        var newRotation = _spawnTransforms[spawnPointIndex].rotation;

        runner.Spawn(_playerPrefab, newPosition, newRotation, runner.LocalPlayer);
    }

    private LocalInputs _localInputs;
    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!NetworkPlayer.Local) return;

        _localInputs ??= NetworkPlayer.Local.LocalInputs;

        input.Set(_localInputs.GetLocalInputs());
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        runner.Shutdown();
    }
    
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){ }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){ }
}
