using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    private PlayerInput _input;
    private PlayerController _controller;

    private InputAction _move;
    private InputAction _point;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _controller = GetComponent<PlayerController>();
        _move = _input.actions["Move"];
        _point = _input.actions["Point"];
    }

    void OnEnable()
    {
        _input?.ActivateInput();
    }

    void OnDisable()
    {
        _input?.DeactivateInput();
    }

    void Start()
    {
        
    }


    void Update()
    {
        Vector2 move2D = _move.ReadValue<Vector2>();
        Vector3 move3D = new Vector3(move2D.x, 0, move2D.y);

        _controller.Move(move3D);

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
    }
}
