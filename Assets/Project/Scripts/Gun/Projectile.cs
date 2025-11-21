using UnityEngine;
using UnityEngine.WSA;

public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;
    [SerializeField]private float _damage = 1;

    private float _speed;
    
    void Update()
    {
        float moveDistance = _speed * Time.deltaTime;
        CheckColision(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckColision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, moveDistance,
            collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }
    
    void OnHitObject(RaycastHit _hit)
    {
        Debug.Log(_hit.collider.gameObject.name);

        if(_hit.collider.TryGetComponent(out IDamageable colliderObject)){
            colliderObject.TakeHit(_damage, _hit);
        }

        Destroy(gameObject);
    }

    public void SetSpeed(float speedvalue)
    {
        _speed = speedvalue;
    }
}
