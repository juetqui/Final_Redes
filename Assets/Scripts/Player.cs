using System;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] int _maxLife;
    [SerializeField] float _shootRate;

    [Networked, OnChangedRender(nameof(CurrentLifeChanged))]
    private int CurrentLife { get; set; }
    [Networked] private TickTimer _shootCooldown { get; set; }


    void CurrentLifeChanged() => Debug.Log(CurrentLife);

    [SerializeField] private LayerMask _shotLayers;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnerTransform;

    private bool _isShootPressed;
    private Vector2 _input;

    private NetworkRigidbody3D _rb;
    public event Action<float> OnMovement = delegate { };
    public event Action OnShot = delegate { };

    [SerializeField] private float _dashForce = 15f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldownTime = 2f;
    [SerializeField] private float _maxTrapDistance = 5f;

    private bool _isDashPressed;

    [Networked] private TickTimer _dashTimer { get; set; }
    [Networked] private TickTimer _dashCooldown { get; set; }

    private Vector3 _lastMoveDirection = Vector3.zero;

    [SerializeField] private NetworkObject _trapPrefab;
    private bool _isSecondaryShotPressed;

    [Networked] private TickTimer _trapTimer { get; set; }
    [Networked] private TickTimer _trapCooldown { get; set; }

    [SerializeField] private int _maxTraps = 2;
    [SerializeField] private float _trapCooldownSeconds = 3f;

    private Camera _mainCam; 

    public override void Spawned()
    {
        _rb = GetComponent<NetworkRigidbody3D>();
        _mainCam = Camera.main;

        CurrentLife = _maxLife;

        if (HasStateAuthority)
        {
            _mainCam.GetComponent<FollowTarget>().SetTarget(this);
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.blue;
        }
        else
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
        }

        GameManager.Instance.AddToList(this);
    }

    void Update()
    {
        if (!HasStateAuthority) return;

        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetMouseButtonDown(0))
        {
            _isShootPressed = true;
        }

        if (Input.GetMouseButtonDown(1)) // botón derecho
        {
            _isSecondaryShotPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isDashPressed = true;
        }

        LookAtMouse();
    }

    public override void FixedUpdateNetwork()
    {
        Move(_input);

        if (_isShootPressed && _shootCooldown.ExpiredOrNotRunning(Runner))
        {
            SpawnShot();
            _shootCooldown = TickTimer.CreateFromSeconds(Runner, 2f);
        }

        if (_isSecondaryShotPressed)
        {
            SpawnTrap();
            _isSecondaryShotPressed = false;
        }

        if (_isDashPressed &&
            _dashCooldown.ExpiredOrNotRunning(Runner) &&
            _trapTimer.ExpiredOrNotRunning(Runner) &&
            _lastMoveDirection.magnitude > 0.1f)
        {
            StartDash();
        }

        _isShootPressed = false;
        _isDashPressed = false;
    }

    void Move(Vector2 input)
    {
        if (!_trapTimer.ExpiredOrNotRunning(Runner))
        {
            _rb.Rigidbody.velocity = Vector3.zero;
            return;
        }

        if (!_dashTimer.ExpiredOrNotRunning(Runner))
        {
            // Si estamos dashing, no modificar la velocidad
            return;
        }

        Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;
        _lastMoveDirection = moveDir;

        _rb.Rigidbody.velocity = moveDir * _speed;
        OnMovement(moveDir.magnitude);
    }


    void LookAtMouse()
    {
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~0))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y; // Mantener altura constante
            transform.LookAt(lookPoint);
        }
    }

    void SpawnShot()
    {
        if (!HasStateAuthority) return; // 

        if (_bulletPrefab == null || _bulletSpawnerTransform == null)
        {
            Debug.LogError("Bullet prefab o spawner transform no asignados.", this);
            return;
        }

        Runner.Spawn(_bulletPrefab, _bulletSpawnerTransform.position, transform.rotation);
        OnShot();
    }

    void SpawnTrap()
    {
        if (!HasStateAuthority || _trapPrefab == null) return;
        if (!_trapCooldown.ExpiredOrNotRunning(Runner)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 targetPoint = hit.point;
            Vector3 direction = (targetPoint - transform.position);
            direction.y = 0;

            float distance = direction.magnitude;

            if (distance > _maxTrapDistance)
            {
                direction = direction.normalized * _maxTrapDistance;
            }

            Vector3 spawnPosition = transform.position + direction;
            spawnPosition.y = 0;

            Runner.Spawn(_trapPrefab, spawnPosition, Quaternion.identity);

            _trapCooldown = TickTimer.CreateFromSeconds(Runner, _trapCooldownSeconds);
        }
    }

    void StartDash()
    {
        _rb.Rigidbody.velocity = _lastMoveDirection.normalized * _dashForce;
        _dashTimer = TickTimer.CreateFromSeconds(Runner, _dashDuration);
        _dashCooldown = TickTimer.CreateFromSeconds(Runner, _dashCooldownTime);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(int dmg)
    {
        Local_TakeDamage(dmg);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ApplyTrap(float duration)
    {
        _trapTimer = TickTimer.CreateFromSeconds(Runner, duration);
    }

    void Local_TakeDamage(int dmg)
    {
        CurrentLife -= dmg;

        if (CurrentLife <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Debug.Log("Mori :(");
        GameManager.Instance.RPC_Defeat(Runner.LocalPlayer);
        Runner.Despawn(Object);
    }
}
