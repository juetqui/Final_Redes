using Fusion;
using UnityEngine;

public class Trap : NetworkBehaviour
{
    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _lifetime = 3f;

    [Networked] private TickTimer _lifeTimer { get; set; }

    public override void Spawned()
    {
        _lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        if (other.TryGetComponent(out Player player))
        {
            player.RPC_ApplyTrap(_duration);
            Runner.Despawn(Object);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
