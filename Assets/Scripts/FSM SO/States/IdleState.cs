
using System;
using UnityEngine;
[CreateAssetMenu(fileName = "IdleState", menuName = "StatesSO/Idle")]
public class IdleState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        if (ec.DamagedHP != 0)
        {
            ec.currentPointIndex = 0;
            ec.GetPatrolRoute(ec.originPatrolPoints, ec.transform.position);
        }
    }

    public override void OnStateExit(EnemyController ec)
    {

    }

    public override void OnStateUpdate(EnemyController ec)
    {
        if (ec.originPatrolPoints.Count > 0)
        {
            if ((ec.originPatrolPoints[ec.currentPointIndex] - ec.transform.position).magnitude < 1)
            {
                ec.currentPointIndex = ec.currentPointIndex + 1 == ec.originPatrolPoints.Count ? 0 : ec.currentPointIndex + 1;
            }

            ec.movementBehavior.GoTo(ec.originPatrolPoints[ec.currentPointIndex], ec.transform);

        }
    }
}
