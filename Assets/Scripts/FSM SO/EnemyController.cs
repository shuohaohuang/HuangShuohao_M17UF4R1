using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private bool onAlert,
        canAttack,
        onAttackRange,
        onVisionRange,
        onCooldown;
    public bool OnAlert
    {
        get => onAlert;
        set
        {
            onAlert = value;

            CheckEndingConditions();
        }
    }
    public bool OnAttackRange
    {
        get => onAttackRange;
        set
        {
            onAttackRange = value;
            CheckEndingConditions();
        }
    }
    public bool OnRange
    {
        get => onVisionRange;
        set
        {
            onVisionRange = value;
            if (!onVisionRange)
                GoLastPoint();
            CheckEndingConditions();
        }
    }
    public bool CanAttack
    {
        get => canAttack;
        set
        {
            canAttack = value;
            CheckEndingConditions();
        }
    }

    public int DamagedHP
    {
        get => damagedHP;
        set
        {
            damagedHP = value;
            CheckEndingConditions();
        }
    }

    public bool OnCooldown
    {
        get => onCooldown;
        set
        {
            onCooldown = value;
            CheckEndingConditions();
        }
    }

    public Animator animator;

    public ChaseBehaviour movementBehavior;
    public Coroutine alertCoroutine;
    public float alertTime;
    public float attackTime;
    public float cd;
    public float currentAttackTime;
    public float currentCd;
    public float maxPatrolDistance = 20;
    public float attackRange;
    public float remainingAlertTime;
    public int currentPointIndex = 0;
    private int damagedHP;
    public int HP;
    public List<StateSO> Nodes;
    public List<Vector3> alertPatrolPoints;
    public List<Vector3> originPatrolPoints;
    public List<Vector3> patrolForwards =
        new()
        {
            new Vector3(0, -2.430758f, 0.707f).normalized,
            new Vector3(0, -2.430758f, -0.707f).normalized,
            new Vector3(-0.707f, -2.430758f, 0).normalized,
            new Vector3(0.707f, -2.430758f, 0).normalized,
        };
    public PCController target;
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

            if (
                Physics.Raycast(
                    startPoint,
                    direction * maxPatrolDistance,
                    out controlRay,
                    maxPatrolDistance
                )
            )
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
            animator.SetTrigger("attack");
            StartCoroutine(Cooldown());
        }
    }

    public void HurtTarget()
    {
        Vector3 direccion = target.transform.position - transform.position;

        //It is also compared with Ground because the raycast bugs with the attack state
        if (
            Physics.Raycast(transform.position, direccion, out controlRay)
            && (
                Equals(controlRay.collider.gameObject, target.gameObject)
                || controlRay.collider.gameObject.layer == LayerMask.NameToLayer("Ground")
            )
            && direccion.magnitude < attackRange + 1f
        )
        {
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
            }
            yield return null;
        }

        CanAttack = true;
    }

    protected IEnumerator Alert()
    {
        while (remainingAlertTime > 0)
        {
            remainingAlertTime -= Time.deltaTime;
            yield return null;
        }
        OnAlert = false;

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

    public void ShufflePoints(List<Vector3> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Vector3 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void CheckRange()
    {
        Vector3 direccion = target.transform.position - transform.position;

        //It is also compared with Ground because the raycast bugs with the attack state
        if (
            Physics.Raycast(transform.position, direccion, out controlRay)
            && (
                Equals(controlRay.collider.gameObject, target.gameObject)
                || controlRay.collider.gameObject.layer == LayerMask.NameToLayer("Ground")
            )
        )
        {
            lastPlayerPosition = new(
                target.transform.position.x,
                transform.position.y,
                target.transform.position.z
            );

            OnRange = true;
            OnAlert = false;

            if (controlRay.distance < attackRange)
            {
                OnAttackRange = true;
            }
            else
            {
                OnAttackRange = false;
            }
        }
        else
        {
            OnRange = false;
        }
        Color color = OnAttackRange ? Color.green : Color.red;
        Debug.DrawLine(transform.position, target.transform.position, color, 0.1f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target.gameObject) { }
    }

    public void OnTriggerStay(Collider other)
    {
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
        }
    }

    public void GoLastPoint()
    {
        if (lastPlayerPosition != Vector3.zero)
        {
            //counter for alert time
            remainingAlertTime = alertTime;
            OnAlert = true;
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
