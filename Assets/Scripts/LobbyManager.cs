using Fusion;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playersConnectedText;
    [SerializeField] private SceneRef gameScene;       // Escena de juego a cargar
    [SerializeField] private SceneRef lobbyScene;      // Escena del lobby (actual)

    private NetworkRunner _Lobbyrunner;

    private void Start()
    {
        _Lobbyrunner = FindObjectOfType<NetworkRunner>();
        if (_Lobbyrunner == null)
        {
            throw new Exception("No se encontró un NetworkRunner en la escena. Asegúrate de que el LobbyManager esté en una escena con un NetworkRunner.");
        }
        UpdateConnectedPlayersText();
    }

    private void UpdateConnectedPlayersText()
    {
        if (_Lobbyrunner != null && playersConnectedText != null)
        {
            int playerCount = _Lobbyrunner.ActivePlayers.Count();
            playersConnectedText.text = $"Jugadores conectados: {playerCount}/2";
            if (playerCount >= 2)
            {
                StartGame(_Lobbyrunner);
            }
        }
    }

    async void StartGame(NetworkRunner _Lobbyrunner)
    {
        await _Lobbyrunner.SceneManager.LoadScene(gameScene, new NetworkLoadSceneParameters());
        await _Lobbyrunner.SceneManager.UnloadScene(lobbyScene);
    }

    // Callbacks para actualizar texto cuando jugadores entran o salen
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        UpdateConnectedPlayersText();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        UpdateConnectedPlayersText();
    }

}
