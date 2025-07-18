using UnityEngine;
using Fusion;

public class PlayerView : NetworkBehaviour
{
    private NetworkMecanimAnimator _mecanim;
    [SerializeField] private GameObject _playerVisual;

    public override void Spawned()
    {
        _mecanim = GetComponentInChildren<NetworkMecanimAnimator>();
        var m = GetComponentInParent<Player>();
        m.OnMovement += MoveAnimation;

        var lifeComponent = GetComponentInParent<LifeHandler>();

        if (lifeComponent)
        {
            lifeComponent.OnDeadChanged += EnableMeshRender;
        }
    }

    void MoveAnimation(Vector2 input)
    {
        _mecanim.Animator.SetFloat("MoveX", input.x);
        _mecanim.Animator.SetFloat("MoveZ", input.y);
    }

    void EnableMeshRender(bool e)
    {
        _playerVisual.SetActive(!e);
    }
}
