using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private int maxThrowDistance = 7;
    [SerializeField] private Transform grenadePrefab;
    public override string GetActionName()
    {
        return "Grenade";
    }
    private void Update()
    {
        if(!isActive)
        {
            return;
        }
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }               

                

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition position, Action onActionComplete)
    {
        Transform grenadeProjectileTranfrom = Instantiate(grenadePrefab, unit.transform.position, Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTranfrom.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(position, OnGrenadeBehaviourComplete);
        Debug.Log("Grenade action");
        ActionStart(onActionComplete);
    }
    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }
}
