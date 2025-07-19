using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunnerHandler _networkRunnerHandler;

    [Header("Panels")]
    [SerializeField] private GameObject _initialPanel;
    [SerializeField] private GameObject _statusPanel;
    [SerializeField] private GameObject _hostGamePanel;

    [Header("Buttons")]
    [SerializeField] private Button _joinLobbyBTN;
    [SerializeField] private Button _hostGameBTN;

    [Header("Texts")]
    [SerializeField] private TMP_Text _statusText;

    void Start()
    {
        _joinLobbyBTN.onClick.AddListener(Btn_JoinLobby);
        _hostGameBTN.onClick.AddListener(Btn_CreateGameSession);

        _networkRunnerHandler.OnJoinedLobby += () =>
        {
            Debug.Log("[Custom Msg] Joined Lobby");
            _statusPanel.SetActive(false);
            _hostGamePanel.SetActive(true);
        };
    }

    void Btn_JoinLobby()
    {
        _networkRunnerHandler.JoinLobby();

        _initialPanel.SetActive(false);
        _statusPanel.SetActive(true);

        _statusText.text = "Joining Lobby...";
    }

    void Btn_CreateGameSession()
    {
        _hostGameBTN.interactable = false;

        _networkRunnerHandler.CreateGame("GameSession", "Game");
    }
}
