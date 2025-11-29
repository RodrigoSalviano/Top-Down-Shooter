using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public enum State {Idle, Chasing, Attacking};

    private NavMeshAgent _agent;
    private Transform _target;
    private State _currentState;
    private Material _skinMaterial;
    private Color _originalColor;
    private LivingEntity _targetEntity;

    [Header("Attack Settings")]
    [SerializeField] private float attackDistanceTreshHold = .5f;
    [SerializeField] private float damage;
    [SerializeField] private float timeBetweenAttacks = 1f;
    [SerializeField] private Color colorOnAttack;

    [Space(10)]
    [Header("Calculation per second (Path)")]
     [SerializeField] private float refreshRate = .25f;
    

    private bool _hasTarget;
    private float _nextAttackTime;
    private float _myCollisionRadius;
    private float _targetCollisionRadius;
    

    protected override void Start()
    {
        base.Start();

        _agent = GetComponent<NavMeshAgent>();
        _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        _skinMaterial = GetComponent<Renderer>().material;
        _originalColor = _skinMaterial.color;

        if(GameObject.FindGameObjectWithTag("Player") !=null)
        {
            _currentState = State.Chasing;
            _hasTarget = true;

            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _targetEntity = _target.GetComponent<LivingEntity>();
            _targetEntity.OnDeath += OnTargetDepth;


            _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }

        

    }

    void Update()
    {
        if (_hasTarget)
        {
            if(Time.time > _nextAttackTime)
            {
                float sqrDistanceToTarget = (_target.position - transform.position).sqrMagnitude;

                if(sqrDistanceToTarget < Mathf.Pow(attackDistanceTreshHold +
                 _myCollisionRadius + _targetCollisionRadius, 2))
                {
                    _nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                } 
            }
        }
    }

    void OnTargetDepth()
    {
        _hasTarget = false;
        _currentState = State.Idle;
    }

    IEnumerator Attack()
    {
        _currentState = State.Attacking;
        _agent.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (_target.position - transform.position).normalized;
        Vector3 attackPosition = _target.position - dirToTarget * _myCollisionRadius;

        float percent = 0;
        float attackSpeed = 3f;
        bool hasAppliedDamage = false;

        _skinMaterial.color = colorOnAttack;

        while(percent <= 1)
        {

            if(percent >= 0.5f && !hasAppliedDamage)
            {
                _targetEntity.TakeDamage(damage);
                hasAppliedDamage = true;
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        _skinMaterial.color = _originalColor;
        _currentState = State.Chasing;
        _agent.enabled = true;

    }

    IEnumerator UpdatePath()
    {
        while(_hasTarget){
            if(_currentState == State.Chasing){
                Vector3 dirToTarget = (_target.position - transform.position).normalized;
                Vector3 targetPositin = _target.position - dirToTarget * 
                (_myCollisionRadius + _targetCollisionRadius + attackDistanceTreshHold/2);

                if (!_dead)
                {
                   _agent.SetDestination(targetPositin);  
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
