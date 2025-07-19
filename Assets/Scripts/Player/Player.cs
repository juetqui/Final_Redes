using System;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // --- CONFIGURACIÓN GENERAL ---
    [SerializeField] private float _speed;
    [SerializeField] private int _maxLife;
    [SerializeField] private float _shootRate = 1f;
    [SerializeField] private float _secShootRate = 2f;

    // --- DISPARO ---
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Bullet _secBulletPrefab;
    [SerializeField] private Transform _bulletSpawnerTransform;
    [SerializeField] private LayerMask _shotLayers;
    [Networked] private TickTimer _shootCooldown { get; set; }
    [Networked] private TickTimer _secShootCooldown { get; set; }
    private bool _isShootPressed;

    // --- TRAMPAS ---
    [SerializeField] private NetworkObject _trapPrefab;
    [SerializeField] private int _maxTraps = 2;
    [SerializeField] private float _trapCooldownSeconds = 3f;
    [SerializeField] private float _maxTrapDistance = 5f;
    [Networked] private TickTimer _trapCooldown { get; set; }
    [Networked] private TickTimer _trapTimer { get; set; }
    private bool _isSecondaryShotPressed;

    // --- DASH ---
    [SerializeField] private float _dashForce = 15f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldownTime = 2f;
    [Networked] private TickTimer _dashCooldown { get; set; }
    [Networked] private TickTimer _dashTimer { get; set; }
    private bool _isTrapPressed;
    private bool _isDashPressed;
    private Vector3 _lastMoveDirection = Vector3.zero;

    // --- VIDA ---
    [Networked, OnChangedRender(nameof(CurrentLifeChanged))]
    private int CurrentLife { get; set; }

    [SerializeField] private HealthBar _healthBarPrefab;
    [SerializeField] private Vector3 _healthBarPosition;
    private HealthBar _healthBarInstance;

    // --- INPUT Y REFERENCIAS ---
    private Vector2 _input;
    private Camera _mainCam;
    private NetworkRigidbody3D _rb;

    // --- EVENTOS ---
    public event Action<Vector2> OnMovement = delegate { };
    public event Action OnShot = delegate { };

    // --- CICLO DE VIDA ---
    public override void Spawned()
    {
        _rb = GetComponent<NetworkRigidbody3D>();
        _mainCam = Camera.main;

        CurrentLife = _maxLife;

        _healthBarInstance = LifeBarHandler.Instance.AddLifeBar(this);

        if (HasStateAuthority)
        {
            if (_mainCam != null)
                _mainCam.GetComponent<FollowTarget>()?.SetTarget(this);

            var smr = GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
                smr.material.color = Color.blue;
        }
        else
        {
            var smr = GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
                smr.material.color = Color.red;
        }

        GameManager.Instance?.AddToList(this);
    }


    void Update()
    {
        if (!HasStateAuthority ||   GameManager.Instance.GameFinished) return;

        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        _isShootPressed = Input.GetMouseButton(0);
        _isSecondaryShotPressed = Input.GetMouseButtonDown(1);
        _isTrapPressed = Input.GetKeyDown(KeyCode.Space);
        _isDashPressed = Input.GetKeyDown(KeyCode.LeftShift);

        LookAtMouse();
    }

    public override void FixedUpdateNetwork()
    {
        Move(_input);

        if (_isShootPressed && _shootCooldown.ExpiredOrNotRunning(Runner))
        {
            SpawnShot();
            _shootCooldown = TickTimer.CreateFromSeconds(Runner, _shootRate);
        }
        
        if (_isSecondaryShotPressed && _secShootCooldown.ExpiredOrNotRunning(Runner))
        {
            SpawnSecShot();
            _secShootCooldown = TickTimer.CreateFromSeconds(Runner, _secShootRate);
        }

        if (_isTrapPressed)
        {
            SpawnTrap();
            _isTrapPressed = false;
        }

        if (_isDashPressed &&
            _dashCooldown.ExpiredOrNotRunning(Runner) &&
            _trapTimer.ExpiredOrNotRunning(Runner) &&
            _lastMoveDirection.magnitude > 0.1f)
        {
            StartDash();
        }

        _isDashPressed = false;
    }

    // --- MOVIMIENTO Y CÁMARA ---
    private void Move(Vector2 input)
    {
        if (!_trapTimer.ExpiredOrNotRunning(Runner))
        {
            _rb.Rigidbody.velocity = Vector3.zero;
            return;
        }

        if (!_dashTimer.ExpiredOrNotRunning(Runner))
        {
            return; // No moverse durante el dash
        }

        Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;
        _lastMoveDirection = moveDir;

        _rb.Rigidbody.velocity = moveDir * _speed;
        OnMovement(new Vector2(moveDir.x, moveDir.z));
    }

    private void LookAtMouse()
    {
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y;
            transform.LookAt(lookPoint);
        }
    }

    private void StartDash()
    {
        _rb.Rigidbody.velocity = _lastMoveDirection.normalized * _dashForce;
        _dashTimer = TickTimer.CreateFromSeconds(Runner, _dashDuration);
        _dashCooldown = TickTimer.CreateFromSeconds(Runner, _dashCooldownTime);
    }

    // --- ACCIONES (DISPARO, TRAMPA) ---
    private void SpawnShot()
    {
        if (!HasStateAuthority) return;

        if (_bulletPrefab == null || _bulletSpawnerTransform == null)
        {
            Debug.LogError("Bullet prefab o spawner transform no asignados.", this);
            return;
        }

        Runner.Spawn(_bulletPrefab, _bulletSpawnerTransform.position, transform.rotation);
        OnShot();
    }
    
    private void SpawnSecShot()
    {
        if (!HasStateAuthority) return;

        if (_secBulletPrefab == null || _bulletSpawnerTransform == null)
        {
            Debug.LogError("Bullet prefab o spawner transform no asignados.", this);
            return;
        }

        Runner.Spawn(_secBulletPrefab, _bulletSpawnerTransform.position, transform.rotation);
        OnShot();
    }

    private void SpawnTrap()
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
                direction = direction.normalized * _maxTrapDistance;

            Vector3 spawnPosition = transform.position + direction;
            spawnPosition.y = 1f;

            Runner.Spawn(_trapPrefab, spawnPosition, Quaternion.identity);
            _trapCooldown = TickTimer.CreateFromSeconds(Runner, _trapCooldownSeconds);
        }
    }

    // --- DAÑO Y VIDA ---
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(int dmg)
    {
        Local_TakeDamage(dmg);
    }

    private void Local_TakeDamage(int dmg)
    {
        if (dmg >= CurrentLife) dmg = CurrentLife;
        
        CurrentLife -= dmg;

        if (CurrentLife <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Debug.LogError("Mori :(");
        LifeBarHandler.Instance.RemoveLifeBar(_healthBarInstance);

        var gameManager = GameManager.Instance;
        
        if (gameManager != null)
            gameManager.RPC_Defeat(Object.StateAuthority);

        Runner.Despawn(Object);
    }

    void CurrentLifeChanged()
    {
        if (_healthBarInstance != null)
            _healthBarInstance.UpdateLife(CurrentLife, _maxLife);
    }

    // --- EFECTOS EXTERNOS ---
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ApplyTrap(float duration)
    {
        _trapTimer = TickTimer.CreateFromSeconds(Runner, duration);
    }
}
