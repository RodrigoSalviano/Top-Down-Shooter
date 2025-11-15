using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{   
    private Rigidbody _rb;
    private Vector3 _moveInput;
    [SerializeField] private float speed;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }


    public void Move(Vector3 direction)
    {
        _moveInput = direction;
    }

    public void LookAt(Vector3 point)
    {
        Vector3 target = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(target);
    }

    void FixedUpdate()
    {
        Vector3 velocity = _moveInput * speed;
        Vector3 newPosition = _rb.position + velocity * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);
    }
}
