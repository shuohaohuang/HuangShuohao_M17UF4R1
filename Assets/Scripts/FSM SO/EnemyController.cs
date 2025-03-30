
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class EnemyController : MonoBehaviour
{
    public bool OnRange = false, OnAttackRange = false;
    public ChaseBehaviour movementBehavior;
    public Coroutine alertCoroutine;
    public float alertTime;
    public float attackTime;
    public float cd;
    public float currentAttackTime;
    public float currentCd;
    public float maxPatrolDistance = 10;
    public float remainingAlertTime;
    public int currentPointIndex = 0;
    public int DamagedHP;
    public int HP;
    public List<StateSO> Nodes;
    public List<Vector3> alertPatrolPoints;
    public List<Vector3> originPatrolPoints;
    public List<Vector3> patrolForwards = new() { new Vector3(0, -0.15f, 0.707f).normalized, new Vector3(0, -0.15f, -0.707f).normalized, new Vector3(-0.707f, -0.15f, 0).normalized, new Vector3(0.707f, -0.15f, 0).normalized, };
    public Player target;
    public RaycastHit controlRay;
    public StateSO currentNode;
    public Vector3 lastPlayerPosition;
    public Vector3 patrolForward = new Vector3(0, -0.15f, 0.707f).normalized;

    public void GetPatrolRoute(List<Vector3> route, Vector3 startPoint)
    {
        route.Clear();
        route.Add(startPoint);

        foreach (Vector3 forward in patrolForwards)
        {
            Vector3 direction = forward.normalized;

            if (Physics.Raycast(startPoint, direction, out controlRay, maxPatrolDistance))
            {
                if (controlRay.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    route.Add(new Vector3(controlRay.point.x, startPoint.y, controlRay.point.z));
                }
            }
        }
    }

    public void Attack()
    {
        if (currentCd <= 0)
        {

            currentCd = cd;
            currentAttackTime = attackTime;
            StartCoroutine(Cooldown());
            target.OnHurt(1);

        }
    }

    protected IEnumerator Cooldown()
    {
        currentCd = cd;
        while (currentCd > 0)
        {
            currentCd -= Time.deltaTime;
            currentAttackTime -= Time.deltaTime;
            yield return null;
        }
    }

    protected IEnumerator Alert()
    {
        remainingAlertTime = alertTime;
        while (remainingAlertTime > 0)
        {
            remainingAlertTime -= Time.deltaTime;
            yield return null;
        }
    }

    public void GetAlert()
    {
        remainingAlertTime = 0;
        alertCoroutine = StartCoroutine(Alert());
    }
    public void Desalert()
    {
        StopCoroutine(alertCoroutine);
    }

    void Awake()
    {
        GetPatrolRoute(originPatrolPoints, transform.position);
    }

    private void CheckRange()
    {
        Vector3 direccion = target.transform.position - transform.position;
        Physics.Raycast(transform.position, direccion, out controlRay)
        ;
        Debug.Log(controlRay.collider?.gameObject?.name);

        //It is also compared with Ground because the raycast bugs with the attack state
        if (Physics.Raycast(transform.position, direccion, out controlRay) && (Equals(controlRay.collider.gameObject, target.gameObject) || controlRay.collider.gameObject.layer == LayerMask.NameToLayer("Ground")))
        {

            Debug.DrawRay(transform.position, direccion, OnRange ? OnAttackRange ? Color.blue : Color.yellow : Color.red);

            OnRange = true;
            lastPlayerPosition = new(target.transform.position.x, transform.position.y, target.transform.position.z);
        }
        else
        {
            OnRange = false;
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target.gameObject)
        {
            if (currentAttackTime <= 0)
            {
                OnAttackRange = true;

            }
            else
            {
                OnAttackRange = false;
            }
            CheckEndingConditions();

        }
    }
    public void OCollisionExit(Collision collision)
    {
        OnAttackRange = false;
        CheckEndingConditions();

    }

    public void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject == target.gameObject);
        if (other.gameObject == target.gameObject)
        {
            CheckRange();
            CheckEndingConditions();

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            OnRange = false;
            CheckEndingConditions();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HP--;
            DamagedHP++;
        }

        currentNode.OnStateUpdate(this);
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
