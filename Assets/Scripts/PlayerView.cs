using UnityEngine;
using Fusion;

[RequireComponent(typeof(Player))]
public class PlayerView : NetworkBehaviour
{
    private NetworkMecanimAnimator _mecanim;
    
    public override void Spawned()
    {
        _mecanim = GetComponentInChildren<NetworkMecanimAnimator>();
        var m = GetComponent<Player>();
        m.OnMovement += MoveAnimation;
    }

    void MoveAnimation(float xAxi)
    {
        _mecanim.Animator.SetFloat("axi", Mathf.Abs(xAxi));
    }
}
