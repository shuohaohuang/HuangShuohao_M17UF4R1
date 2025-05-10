using UnityEngine;
[CreateAssetMenu(fileName = "AlertState", menuName = "StatesSO/AlertState")]
public class AlertState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        if (ec.alertCoroutine == null) ec.GetAlert();
        ec.GetPatrolRoute(ec.alertPatrolPoints, ec.lastPlayerPosition);
        ec.currentPointIndex = 0;
    }

    public override void OnStateExit(EnemyController ec)
    {
    }

    public override void OnStateUpdate(EnemyController ec)
    {
        if (ec.alertPatrolPoints.Count > 0)
        {
            if ((ec.alertPatrolPoints[ec.currentPointIndex] - ec.transform.position).magnitude < 1)
            {
                ec.currentPointIndex = ec.currentPointIndex + 1 == ec.alertPatrolPoints.Count ? 0 : ec.currentPointIndex + 1;
            }

            ec.movementBehavior.GoTo(ec.alertPatrolPoints[ec.currentPointIndex], ec.transform);

        }
    }
}
