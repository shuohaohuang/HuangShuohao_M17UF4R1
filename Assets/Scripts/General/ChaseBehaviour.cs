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

        agent.stoppingDistance = 0.1f;
        agent.acceleration = 20f;
        agent.autoBraking = false;
    }
    public void Chase(Transform target, Transform self)
    {
        agent.isStopped = false;
        agent.SetDestination(new(target.position.x, self.position.y, target.position.z));
    }
    public void Run(Transform target, Transform self)
    {
        agent.isStopped = false;

        Vector3 runDirection = (self.position - target.position).normalized;
        runDirection.y = 0;
        Vector3 runPoint = self.position + runDirection * 1;

        agent.SetDestination(runPoint);

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
