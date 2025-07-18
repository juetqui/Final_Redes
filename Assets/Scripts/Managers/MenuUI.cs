using Fusion;

public class MenuUI : NetworkBehaviour
{
    public void OnClickStartGame()
    {
        NetworkGameManager.Instance.StartSession(isHost: true);
    }
}
