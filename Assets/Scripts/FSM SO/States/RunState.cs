using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RunState", menuName = "StatesSO/Run")]
public class RunState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        ec.GetComponent<ChaseBehaviour>().Speed *= 3;
        Debug.Log("HUYO");
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
