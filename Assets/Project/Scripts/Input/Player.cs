using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerController))]
public class Player : LivingEntity
{
    private PlayerInput _input;
    private PlayerController _controller;
    private GunController _gunController;
    

    #region Input Actions
    private InputAction _move;
    private InputAction _point;
    private InputAction _shoot;
    #endregion

    [SerializeField] private Animator _animator;
    [SerializeField] private float timeBetweenEmote = 5f;

    private float emoteTime;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _controller = GetComponent<PlayerController>();
        _gunController = GetComponent<GunController>();
        _move = _input.actions["Move"];
        _point = _input.actions["Point"];
        _shoot = _input.actions["Shoot"];
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

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if(groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 worldPoint = ray.GetPoint(rayDistance);

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            //Gizmos.DrawSphere(worldPoint, .3f);

            _controller.LookAt(worldPoint);
        }

        if (_shoot.IsPressed())
        {
            if(_gunController.Shoot()){
            _controller.Recoil(_gunController.recoilForce);
            }
        }
    
    }
}
