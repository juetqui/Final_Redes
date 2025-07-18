using UnityEngine;
using Fusion;
using System.Linq;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _gameManagerPrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnTransforms;

    private bool _initialized;

    private void Start()
    {
        if (Runner != null)
        {
            Runner.AddCallbacks((INetworkRunnerCallbacks)this);

            // Forzar el spawn de jugadores ya conectados
            foreach (var player in Runner.ActivePlayers)
            {
                PlayerJoined(player);
            }
        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer && GameManager.Instance == null)
        {
            Runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity);
        }

        var playersCount = Runner.ActivePlayers.Count();

        if (_initialized && playersCount >= 2)
        {
            CreatePlayer(0);
            return;
        }

        if (player == Runner.LocalPlayer)
        {
            if (playersCount < 2)
            {
                _initialized = true;
            }
            else
            {
                CreatePlayer(playersCount - 1);
            }
        }
    }

    void CreatePlayer(int spawnPointIndex)
    {
        _initialized = false;
        spawnPointIndex = spawnPointIndex % _spawnTransforms.Length;
        var newPosition = _spawnTransforms[spawnPointIndex].position;
        var newRotation = _spawnTransforms[spawnPointIndex].rotation;

        Runner.Spawn(_playerPrefab, newPosition, newRotation, Runner.LocalPlayer);
    }
}
