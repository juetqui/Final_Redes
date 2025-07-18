using Fusion;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbyUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playersConnectedText;
    [SerializeField] private SceneRef gameScene;
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

    private async void UpdateConnectedPlayersText()
    {
        if (_Lobbyrunner != null && playersConnectedText != null)
        {
            int playerCount = _Lobbyrunner.ActivePlayers.Count();
            playersConnectedText.text = $"Jugadores conectados: {playerCount}/2";
            Debug.Log("entre");
            if (playerCount >= 2)
            {
                Debug.Log("igual o mas de dos");
                await NetworkGameManager.Instance.LoadSceneAsync(gameScene);
            }
        }
    }

    // Callbacks para actualizar texto cuando jugadores entran o salen
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Jugador {player} se ha unido.");
        UpdateConnectedPlayersText();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        UpdateConnectedPlayersText();
    }
}
