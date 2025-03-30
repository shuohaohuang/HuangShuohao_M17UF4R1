using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DieState", menuName = "StatesSO/Die")]
public class DieState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        ec.movementBehavior.agent.enabled = false;
        ec.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public override void OnStateExit(EnemyController ec)
    {
    }

    public override void OnStateUpdate(EnemyController ec)
    {
        // GameManager.gm.UpdateText("Abandon� este mundo de miseria y desesperaci�n");
    }
}
