using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] private float startingHealth = 5f;

    private float _health;
    protected bool _dead;

    public event Action OnDeath;

    protected virtual void Start()
    {
        _health = startingHealth;
    }

    public virtual void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
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
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}