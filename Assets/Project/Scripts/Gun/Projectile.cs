using UnityEngine;
using UnityEngine.WSA;

public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;
    [SerializeField]private float _damage = 1;
    [SerializeField] private float lifetime;

    private float _speed;
    private float _targetBoardSize = .1f;

    void Start()
    {
        Destroy(gameObject, lifetime);

        Collider[] InitialCollsions = Physics.OverlapSphere(transform.position, .1f, collisionMask);

        if(InitialCollsions.Length > 0)
        {
            OnHitObject(InitialCollsions[0]);
        }
    }

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
        
        if(Physics.Raycast(ray, out hit, moveDistance + _targetBoardSize,
            collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }
    
    void OnHitObject(RaycastHit _hit)
    {
        //Debug.Log(_hit.collider.gameObject.name);

        if(_hit.collider.TryGetComponent(out IDamageable colliderObject)){
            colliderObject.TakeHit(_damage, _hit);
        }

        Destroy(gameObject);
    }

    void OnHitObject(Collider collider)
    {
        if(collider.TryGetComponent(out IDamageable colliderObject))
        {
            colliderObject.TakeDamage(_damage);
        }

        Destroy(gameObject);   
    }

    public void SetSpeed(float speedvalue)
    {
        _speed = speedvalue;
    }
}
