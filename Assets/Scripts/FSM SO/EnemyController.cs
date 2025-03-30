

using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class EnemyController : MonoBehaviour
{
    public bool OnRange = false, OnAttackRange = false, onAlert = false;
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
        ShufflePoints(route);
    }

    public void Attack()
    {
        if (currentCd <= 0)
        {
            StartCoroutine(Cooldown());
            target.OnHurt(1);
        }
    }

    protected IEnumerator Cooldown()
    {
        currentCd = cd;
        currentAttackTime = attackTime;
        while (currentCd > 0)
        {
            currentCd -= Time.deltaTime;
            currentAttackTime -= Time.deltaTime;

            if (currentAttackTime < 0)
            {
                OnAttackRange = false;
                //while currentAttackTime is greatter than 0 the enemy cannot move
                //it to trigger chase state while attack is on cooldown
                CheckEndingConditions();
            }
            yield return null;
        }
    }

    protected IEnumerator Alert()
    {
        while (remainingAlertTime > 0)
        {
            remainingAlertTime -= Time.deltaTime;
            yield return null;
        }
        onAlert = false;

        // it to trigger idle
        CheckEndingConditions();

        //desactive corroutine for next alert state 
        Desalert();
    }

    public void GetAlert()
    {
        alertCoroutine = StartCoroutine(Alert());
    }

    public void Desalert()
    {
        remainingAlertTime = 0;
        StopCoroutine(alertCoroutine);
        alertCoroutine = null;
    }

    void Awake()
    {
        GetPatrolRoute(originPatrolPoints, transform.position);
    }
    void ShufflePoints(List<Vector3> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            Vector3 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    private void CheckRange()
    {
        Vector3 direccion = target.transform.position - transform.position;

        //It is also compared with Ground because the raycast bugs with the attack state
        if (Physics.Raycast(transform.position, direccion, out controlRay) && (Equals(controlRay.collider.gameObject, target.gameObject) || controlRay.collider.gameObject.layer == LayerMask.NameToLayer("Ground")))
        {

            Debug.DrawRay(transform.position, direccion, Color.yellow);

            OnRange = true;
            onAlert = false;
            CheckEndingConditions();

            lastPlayerPosition = new(target.transform.position.x, transform.position.y, target.transform.position.z);
        }
        else
        {

            OnRange = false;

            GoLastPoint();

        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target.gameObject)
        {

            OnAttackRange = true;
            //To active attack
            CheckEndingConditions();
        }

    }
    public void OCollisionExit(Collision collision)
    {
        OnAttackRange = false;
        //To active chase
        CheckEndingConditions();

    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            CheckRange();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            OnRange = false;
            GoLastPoint();
            //To active alert, idle is not possible
            CheckEndingConditions();
        }
    }
    public void GoLastPoint()
    {
        //goes to last known position of the player

        if (lastPlayerPosition != Vector3.zero)
        {
            //counter for alert time
            remainingAlertTime = alertTime;
            onAlert = true;

            //lastPlayerPosition != Vector3.zero for an unknown reason doesn't work properly 
            //this can prevent previus bug
            CheckEndingConditions();
            //without this the enemy will chase the player even he is not hitted by raycast
            //it still works if player leaves the detection area.

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
