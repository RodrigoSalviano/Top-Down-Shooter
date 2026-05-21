using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerController))]
public class Player : LivingEntity
{   
    [SerializeField] private Crosshair crosshair;

    private PlayerInput _input;
    private PlayerController _controller;
    private GunController _gunController;
    

    #region Input Actions
    private InputAction _move;
    private InputAction _point;
    private InputAction _shoot;
    private InputAction _reload;
    #endregion

    #region Debug Session
    private InputAction _debugNextWave;
    public event Action OnDebugNextWave;
    #endregion

    [SerializeField] private Animator _animator;
    [SerializeField] private float timeBetweenEmote = 5f;

    [Header("Debug Session")]
    [SerializeField] private bool debugToolsEnable;

    private float emoteTime;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _controller = GetComponent<PlayerController>();
        _gunController = GetComponent<GunController>();
        _move = _input.actions["Move"];
        _point = _input.actions["Point"];
        _shoot = _input.actions["Shoot"];
        _reload = _input.actions["Reload"];

        _debugNextWave = _input.actions["Debug_NextWave"];
        crosshair = Instantiate(crosshair, Vector3.zero, crosshair.transform.rotation);

        FindAnyObjectByType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnEnable()
    {
        _input?.ActivateInput();
    }

    void OnDisable()
    {
        _input?.DeactivateInput();
    }

    void Update()
    {
        Vector2 move2D = _move.ReadValue<Vector2>();
        Vector3 move3D = new Vector3(move2D.x, 0, move2D.y);

        _controller.Move(move3D);
        _animator.SetFloat("speed", move3D.magnitude);

        if (move3D.magnitude <= 0)
        {
            emoteTime -= Time.deltaTime;

            if(emoteTime <= 0){
                emoteTime += timeBetweenEmote;
                _animator.SetBool("Emote_1", true);
            }
            else
            {
            _animator.SetBool("Emote_1", false);
            }
        }
        else
        {
            _animator.SetBool("Emote_1", false);
        }
        

        Vector2 screenPoint = _point.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);

        Plane groundPlane = new Plane(Vector3.up, Vector3.up * 1f);

        if(groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 worldPoint = ray.GetPoint(rayDistance);

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            //Gizmos.DrawSphere(worldPoint, .3f);

            float distanceToCrosshair = (new Vector2(worldPoint.x, worldPoint.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude;
            _controller.LookAt(worldPoint);

            if(distanceToCrosshair > 1.1f)
            {
            _gunController.SetRotation(worldPoint);
            }

            crosshair.transform.position = worldPoint;
            crosshair.OnTargetDetect(ray);
        }

       
        if (_shoot.IsPressed())
        {
            if(_gunController.OnTriggerHold()){
            _controller.Recoil(_gunController.RecoilForce);
            }
        }

        if (_reload.WasPressedThisFrame())
        {
            _gunController.Reload();
        }

        if (_shoot.WasReleasedThisFrame())
        {
            _gunController.OnTriggerRealease();
        }

        if(debugToolsEnable && _debugNextWave.WasPressedThisFrame())
        {
            OnDebugNextWave?.Invoke();
        }
    }

    private void OnNewWave(int waveIndex)
    {
        _health = startingHealth;
        _gunController.EquipGun(waveIndex - 1);
    }

     public override void Die()
    {
        base.Die();
        Cursor.visible = true;
    }
}
