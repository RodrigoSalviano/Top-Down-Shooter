using UnityEngine;
using UnityEngine.WSA;

public class Projectile : MonoBehaviour
{
    private float _speed;


    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    public void SetSpeed(float speedvalue)
    {
        _speed = speedvalue;
    }
}
