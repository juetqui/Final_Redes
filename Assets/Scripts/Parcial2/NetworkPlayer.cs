using System;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(LocalInputs))]
public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; private set; }
    public LocalInputs LocalInputs { get; private set; }
    
    public event Action OnLeft = delegate { };

    public override void Spawned()
    {
        LocalInputs = GetComponent<LocalInputs>();
        
        if (Object.HasInputAuthority)
        {
            Local = this;
            LocalInputs.enabled = true;
        }
        else
        {
            LocalInputs.enabled = false;
        }
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnLeft();
    }
}
