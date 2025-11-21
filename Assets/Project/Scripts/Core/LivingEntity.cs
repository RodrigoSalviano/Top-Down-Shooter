using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] private float startingHealth = 5f;

    private float _health;
    protected bool _dead;

    protected virtual void Start()
    {
        _health = startingHealth;
    }

    public virtual void TakeHit(float damage, RaycastHit hit)
    {
        _health -= damage;

        if(_health <= 0 && !_dead)
        {
            Die();
        }
    }

    private void Die()
    {
        _dead = true;
        Destroy(gameObject);
    }
}