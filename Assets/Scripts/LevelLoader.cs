using Fusion;
using UnityEngine;

public class MenuManager : NetworkBehaviour
{
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private SceneRef lobbyScene; // Asignar desde el Inspector

    private NetworkRunner _runner;

    public void OnClick_Start()
    {
        StartGame();
    }

    async void StartGame()
    {
        _runner = Instantiate(runnerPrefab);
        DontDestroyOnLoad(_runner.gameObject);

        _runner.ProvideInput = true;

        var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        //_runner.AddCallbacks(this);

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "Sala_01",
            SceneManager = sceneManager
        });

        // Ahora cargás el Lobby manualmente
        if (result.Ok)
        {
            await _runner.SceneManager.LoadScene(lobbyScene, new NetworkLoadSceneParameters());
        }
        else
        {
            Debug.LogError($"Error al iniciar partida: {result.ShutdownReason}");
        }
    }

}
