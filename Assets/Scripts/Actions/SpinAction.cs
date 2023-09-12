using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount = 0;
    private void Update()
    {
        if (!isActive)
            return;

        float spinAmount = 360*Time.deltaTime;
        totalSpinAmount += spinAmount;
        transform.eulerAngles += new Vector3(0, spinAmount, 0);
        if (totalSpinAmount >= 360)
        {
            ActionComplete();
        }
    }   

    public override string GetActionName()
    {
        return "Spin";
    }

    public override void TakeAction(GridPosition position, Action onActionComplete)
    {
        totalSpinAmount = 0;
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition> { unitGridPosition };
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
}
