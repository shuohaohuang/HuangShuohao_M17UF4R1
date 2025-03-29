
using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool OnRange = false, OnAttackRange = false;
    public ChaseBehaviour _chaseB;
    public float maxPatrolditance = 10;
    public float visionRange = 4;
    public GameObject target;
    public int currentPoint = 0;
    public int DamagedHP;
    public int HP;
    public List<StateSO> Nodes;
    public List<Vector3> patrolForwards = new() { new Vector3(0, -0.15f, 0.707f).normalized, new Vector3(0, -0.15f, -0.707f).normalized, new Vector3(-0.707f, -0.15f, 0).normalized, new Vector3(0.707f, -0.15f, 0).normalized, };
    public List<Vector3> patrolPoints;
    public RaycastHit controlRay;
    public StateSO currentNode;
    public Vector3 patrolForward = new Vector3(0, -0.15f, 0.707f).normalized;
    public void GetNewPatrolRoute()
    {
        patrolPoints.Clear();
        patrolPoints.Add(transform.position);

        foreach (Vector3 forward in patrolForwards)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(forward), out controlRay, maxPatrolditance))
            {
                if (controlRay.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    patrolPoints.Add(new(controlRay.point.x, transform.position.y, controlRay.point.z));
                }
            }
        }

    }
    void Start()
    {
        GetNewPatrolRoute();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (target == collision.gameObject)
        {
            OnAttackRange = true;
            CheckEndingConditions();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (target == collision.gameObject)
        {
            OnAttackRange = false;
            CheckEndingConditions();
        }
    }

    private void CheckOnRange()
    {
        Vector3 direccion = (target.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, direccion, out controlRay) && controlRay.collider.gameObject == target)
        {

            OnRange = controlRay.distance < visionRange;
            Debug.DrawRay(transform.position, direccion * controlRay.distance, OnRange ? Color.red : Color.blue);

        }
        else
        {
            OnRange = false;
        }
        // CheckEndingConditions();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HP--;
            DamagedHP++;
            // CheckEndingConditions();
            GetNewPatrolRoute();
        }

        CheckOnRange();
        currentNode.OnStateUpdate(this);

        CheckEndingConditions();
    }
    public void CheckEndingConditions()
    {
        foreach (ConditionSO condition in currentNode.EndConditions)
            if (condition.CheckCondition(this) == condition.answer)
            {
                ExitCurrentNode();
            }
    }
    public void ExitCurrentNode()
    {
        foreach (StateSO stateSO in Nodes)
        {
            if (stateSO.StartCondition == null)
            {
                EnterNewState(stateSO);
                break;
            }
            else
            {
                if (stateSO.StartCondition.CheckCondition(this) == stateSO.StartCondition.answer)
                {
                    EnterNewState(stateSO);
                    break;
                }
            }
        }
        currentNode.OnStateEnter(this);
    }
    private void EnterNewState(StateSO state)
    {
        currentNode.OnStateExit(this);
        currentNode = state;
        currentNode.OnStateEnter(this);
    }
}
