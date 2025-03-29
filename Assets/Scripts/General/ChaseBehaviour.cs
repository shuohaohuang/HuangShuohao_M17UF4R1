using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ChaseBehaviour : MonoBehaviour
{
    public float Speed;
    private Rigidbody _rb;
    public NavMeshAgent agent;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        agent.speed = Speed;
    }
    public void Chase(Transform target, Transform self)
    {
        agent.isStopped = false;
        agent.SetDestination(new(target.position.x, self.position.y, target.position.z));
    }
    public void Run(Transform target, Transform self)
    {
        agent.isStopped = false;
        agent.SetDestination(new(-target.position.x, self.position.y, -target.position.z));

    }

    public void GoTo(Vector3 position, Transform self)
    {
        agent.isStopped = false;
        agent.SetDestination(new(position.x, self.position.y, position.z));
    }
    public void StopChasing()
    {
        agent.isStopped = true;

    }
}
