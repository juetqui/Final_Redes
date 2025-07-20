using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunnerHandler _networkRunnerHandler;

    [Header("Panels")]
    [SerializeField] private GameObject _initialPanel;
    [SerializeField] private GameObject _hostGamePanel;
    [SerializeField] private GameObject[] _playersPannel;

    [Header("Buttons")]
    [SerializeField] private Button _joinLobbyBTN;
    [SerializeField] private Button _hostGameBTN;

    [Header("Texts")]
    [SerializeField] private TMP_Text _statusText;
    int numberPlaye=0;
    void Start()
    {
        _joinLobbyBTN.onClick.AddListener(Btn_JoinLobby);
        _hostGameBTN.onClick.AddListener(Btn_CreateGameSession);
        _hostGameBTN.enabled = false;
        _networkRunnerHandler.OnJoinedLobby += () =>
        {
            Debug.Log("[Custom Msg] Joined Lobby");
            var playersCount = _networkRunnerHandler._runnerPrefab.ActivePlayers;
            _hostGamePanel.SetActive(true);
            _statusText.text = "Start Game";
            _playersPannel[numberPlaye].SetActive(true);
            _hostGameBTN.enabled = true;
            numberPlaye++;
        };
    }

    void Btn_JoinLobby()
    {
        _networkRunnerHandler.JoinLobby();

        _initialPanel.SetActive(false);
        _hostGamePanel.SetActive(true);
        _statusText.text = "Joining Lobby...";
    }

    void Btn_CreateGameSession()
    {
        _hostGameBTN.interactable = false;

        _networkRunnerHandler.CreateGame("GameSession", "Game");
    }
}
