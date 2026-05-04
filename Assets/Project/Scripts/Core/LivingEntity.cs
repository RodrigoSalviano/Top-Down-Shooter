using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 5f;

    protected float _health;
    protected bool _dead;

    public event Action OnDeath;

    protected virtual void Start()
    {
        _health = startingHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
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

    [ContextMenu("Die")]
    public virtual void Die()
    {
        _dead = true;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}