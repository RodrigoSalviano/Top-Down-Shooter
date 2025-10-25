using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls _playerControls;
    private Vector2 _moveInput;

    void Awake()
    {
        _playerControls = new PlayerControls();
        
    }

    void OnEnable()
    {
         _playerControls.Enable(); 
    }

    void OnDisable()
    {
        _playerControls.Disable();
    }

    void Start()
    {
        
    }


    void Update()
    {
        _moveInput = _playerControls.Gameplay.Move.ReadValue<Vector2>();
        Debug.Log(_moveInput);
    }
}
