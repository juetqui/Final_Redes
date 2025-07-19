using System;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(LocalInputs))]
public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; private set; }
    public LocalInputs LocalInputs { get; private set; }
    
    [Networked]
    private NetworkString<_16> Nickname { get; set; }

    private ChangeDetector _changeDetector;
    
    public event Action OnLeft = delegate { };

    public override void Spawned()
    {
        LocalInputs = GetComponent<LocalInputs>();

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        
        if (Object.HasInputAuthority)
        {
            Local = this;
            LocalInputs.enabled = true;

            NetworkString<_16> loadedNick;

            if (PlayerPrefs.HasKey("Nickname"))
            {
                loadedNick = PlayerPrefs.GetString("Nickname");
            }
            else
            {
                loadedNick = $"Player {Runner.LocalPlayer.PlayerId}";
            }
            
            //loadedNick = PlayerPrefs.HasKey("Nickname") ? PlayerPrefs.GetString("Nickname") : "Player";

            RPC_SetNickname(loadedNick);
        }
        else
        {
            LocalInputs.enabled = false;
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_SetNickname(NetworkString<_16> newNickname)
    {
        Nickname = newNickname;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnLeft();
    }
}
