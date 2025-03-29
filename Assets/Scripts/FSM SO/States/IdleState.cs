using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "IdleState", menuName = "StatesSO/Idle")]
public class IdleState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        if (ec.DamagedHP != 0)
        {
            ec.GetNewPatrolRoute();
        }
    }

    public override void OnStateExit(EnemyController ec)
    {
    }

    public override void OnStateUpdate(EnemyController ec)
    {
        if (ec.patrolPoints.Count > 0)
        {
            if ((ec.patrolPoints[ec.currentPoint] - ec.transform.position).magnitude < 1)
            {
                ec.currentPoint = ec.currentPoint + 1 == ec.patrolPoints.Count ? 0 : ec.currentPoint + 1;
            }

            ec._chaseB.GoTo(ec.patrolPoints[ec.currentPoint], ec.transform);

        }
    }
}
