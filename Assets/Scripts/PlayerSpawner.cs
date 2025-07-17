using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnTransforms;

    private bool _initialized;
    
    public void PlayerJoined(PlayerRef player)
    {
        var playersCount = Runner.SessionInfo.PlayerCount;
        
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
        spawnPointIndex = spawnPointIndex % 2 != 0 ? 0 : 1;
        var newPosition = _spawnTransforms[spawnPointIndex].position;
        var newRotation = _spawnTransforms[spawnPointIndex].rotation;
        
        Runner.Spawn(_playerPrefab, newPosition, newRotation);
    }
}
