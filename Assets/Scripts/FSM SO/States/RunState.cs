using UnityEngine;
[CreateAssetMenu(fileName = "RunState", menuName = "StatesSO/Run")]
public class RunState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        ec.GetComponent<ChaseBehaviour>().Speed = 1.2f;

        ec.GetComponent<ChaseBehaviour>().agent.speed = ec.GetComponent<ChaseBehaviour>().Speed;
    }

    public override void OnStateExit(EnemyController ec)
    {
        ec.GetComponent<ChaseBehaviour>().StopChasing();
    }

    public override void OnStateUpdate(EnemyController ec)
    {
        ec.GetComponent<ChaseBehaviour>().Run(ec.target.transform, ec.transform);
    }
}
