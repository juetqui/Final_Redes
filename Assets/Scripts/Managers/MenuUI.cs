using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : NetworkBehaviour
{
    [SerializeField] private Button _PlayBtn;
    public void OnClickStartGame()
    {
        NetworkGameManager.Instance.StartSession(isHost: true);
        _PlayBtn.interactable = false;
    }

    public void OnMenuBTN()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
