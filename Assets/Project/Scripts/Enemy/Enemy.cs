using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    private NavMeshAgent _agent;
    private Transform _target;

    [Header("Calculation per second (Path)")]
    [SerializeField] private float refreshRate = .25f;

    protected override void Start()
    {
        base.Start();
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        while(_target != null && !_dead){
            _agent.SetDestination(_target.position); 
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
