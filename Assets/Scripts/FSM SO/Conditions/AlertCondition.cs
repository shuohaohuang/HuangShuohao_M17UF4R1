using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "OnAlertCondition", menuName = "ConditionSO/AlertCondition")]
public class AlertCondition : ConditionSO
{
    public override bool CheckCondition(EnemyController ec)
    {
        return ec.onAlert;
    }

}
