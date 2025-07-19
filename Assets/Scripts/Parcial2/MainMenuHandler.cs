using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunnerHandler _networkRunnerHandler;

    [Header("Panels")]
    [SerializeField] private GameObject _initialPanel;
    [SerializeField] private GameObject _hostGamePanel;
    //[SerializeField] private GameObject _statusPanel;
    //[SerializeField] private GameObject _sessionBrowserPanel;

    [Header("Buttons")]
    [SerializeField] private Button _joinLobbyBTN;
    [SerializeField] private Button _hostGameBTN;
    //[SerializeField] private Button _hostPanelBTN;

    //[Header("InputFields")] 
    //[SerializeField] private TMP_InputField _sessionName;
    //[SerializeField] private TMP_InputField _nicknameField;

    //[Header("Texts")]
    //[SerializeField] private TMP_Text _statusText;

    void Start()
    {
        _joinLobbyBTN.onClick.AddListener(Btn_JoinLobby);
        _hostGameBTN.onClick.AddListener(Btn_CreateGameSession);
        //_hostPanelBTN.onClick.AddListener(Btn_ShowHostPanel);

        //_networkRunnerHandler.OnJoinedLobby += () =>
        //{
        //    _statusPanel.SetActive(false);
        //    _sessionBrowserPanel.SetActive(true);
        //};
    }

    void Btn_JoinLobby()
    {
        _networkRunnerHandler.JoinLobby();
        _initialPanel.SetActive(false);
        
        //PlayerPrefs.SetString("Nickname", _nicknameField.text);
        //_statusPanel.SetActive(true);
        //_statusText.text = "Joining Lobby...";
    }

    //void Btn_ShowHostPanel()
    //{
    //    _sessionBrowserPanel.SetActive(false);
    //    _hostGamePanel.SetActive(true);
    //}

    void Btn_CreateGameSession()
    {
        _hostGameBTN.interactable = false;

        _networkRunnerHandler.CreateGame("GameSession", "Game");
    }
}
