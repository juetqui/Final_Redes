using Fusion;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private int minPlayersToStart = 2;
    [SerializeField] private SceneRef gameScene; // Asignar en inspector

    private NetworkRunner _runner;

    private void Start()
    {
        _runner = FindObjectOfType<NetworkRunner>();

        if (_runner == null)
        {
            Debug.LogError("No se encontró el NetworkRunner.");
            return;
        }

        if (_runner.IsSharedModeMasterClient)
        {
            InvokeRepeating(nameof(CheckPlayers), 1f, 1f);
        }
    }

    private void CheckPlayers()
    {
        if (_runner.SessionInfo.PlayerCount >= minPlayersToStart)
        {
            Debug.Log("Suficientes jugadores conectados. Iniciando partida...");
            CancelInvoke(nameof(CheckPlayers));

            // Carga sincronizada de la escena
            _runner.SceneManager.LoadScene(gameScene, new NetworkLoadSceneParameters());
        }
    }
}
