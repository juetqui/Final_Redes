using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _initialForce;
    [SerializeField] private int _damage;
    [SerializeField] private float _lifeTime;

    private TickTimer _lifeTimer;

    public override void Spawned()
    {
        var rb = GetComponent<NetworkRigidbody3D>().Rigidbody;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        rb.AddForce(transform.forward * _initialForce, ForceMode.VelocityChange);

        _lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (!_lifeTimer.Expired(Runner)) return;

        Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;


        if (other.TryGetComponent(out Player player))
        {
            player.RPC_TakeDamage((byte)20);
        }
        

        Runner.Despawn(Object);
    }
}
