using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance;

    [Header("General")]
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject PlayerSpawnerPrefab;

    [Header("Scene References")]
    [SerializeField] private SceneRef mainMenuScene;
    [SerializeField] private SceneRef lobbyScene;
    [SerializeField] private SceneRef gameScene;

    private NetworkRunner _runner;
    private bool _isHost;
    private bool _initialized;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public async void StartSession(bool isHost)
    {
        if(_isHost == false)
            _isHost = isHost;

        if (_runner == null)
        {
            _runner = Instantiate(runnerPrefab);
            _runner.name = "NetworkRunner";
            DontDestroyOnLoad(_runner.gameObject);
        }

        _runner.ProvideInput = true;

        var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
            sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "Sala_01",
            SceneManager = sceneManager
        });

        if (result.Ok)
        {
            await LoadSceneAsync(lobbyScene);
        }
        else
        {
            Debug.LogError($"Error al iniciar la sesión: {result.ShutdownReason}");
        }
    }


    public async Task LoadSceneAsync(SceneRef targetScene)
    {
        if (_runner == null || _runner.SceneManager == null)
        {
            Debug.LogError("Runner o SceneManager no están inicializados");
            return;
        }

        // Cargar la nueva escena
        await _runner.SceneManager.LoadScene(targetScene, new NetworkLoadSceneParameters());

        // Una vez la escena esté cargada
        if (targetScene == gameScene)
        {
            // Instanciar el PlayerSpawner
            _runner.Spawn(PlayerSpawnerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("[NetworkGameManager] PlayerSpawner spawned.");
        }

        // Descargar la escena anterior si todavía está cargada
        if (targetScene == lobbyScene || targetScene == gameScene)
        {
            SceneRef prevScene = targetScene == lobbyScene ? mainMenuScene : lobbyScene;
            if (SceneManager.GetSceneByBuildIndex(prevScene.AsIndex).isLoaded)
                await _runner.SceneManager.UnloadScene(prevScene);
        }
    }
    //public async Task LoadSceneAsync(SceneRef targetScene)
    //{
    //    if (_runner == null || _runner.SceneManager == null)
    //    {
    //        Debug.LogError("Runner o SceneManager no están inicializados");
    //        return;
    //    }

    //    if (targetScene == lobbyScene)
    //    {
    //        await _runner.SceneManager.LoadScene(targetScene, new NetworkLoadSceneParameters());

    //        if (SceneManager.GetSceneByBuildIndex(mainMenuScene.AsIndex).isLoaded)
    //        {
    //            await _runner.SceneManager.UnloadScene(mainMenuScene);
    //        }
    //    }

    //    if (targetScene == gameScene)
    //    {
    //        if (_runner.SceneManager.LoadScene(targetScene, new NetworkLoadSceneParameters()).IsDone) 
    //        {
    //            _runner.Spawn(PlayerSpawnerPrefab, Vector3.zero, Quaternion.identity);
    //            Debug.Log("PlayerSpawner spawned successfully.");
    //        }

    //        if (SceneManager.GetSceneByBuildIndex(lobbyScene.AsIndex).isLoaded)
    //        {
    //            await _runner.SceneManager.UnloadScene(lobbyScene);
    //        }
    //    }
    //}   
}
