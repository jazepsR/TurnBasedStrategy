using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction
{
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
        AimingConfirmed,
        Idle,
    }
    public event EventHandler<OnShootEventArgs> OnShoot;
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public class OnShootEventArgs: EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
        public bool hit;
    }
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxShootDistance = 7;
    private State state = State.Idle;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;
    public override string GetActionName()
    {
        return "Shoot";
    }

    protected override void Awake()
    {
        base.Awake();   
        ConfirmActionUI.OnAnyActionConfirm += OnActionConfirm;
        ConfirmActionUI.OnAnyActionCancel += ConfirmActionUI_OnAnyActionCancel;
    }

    private void ConfirmActionUI_OnAnyActionCancel(object sender, EventArgs e)
    {
        if (state == State.Aiming)
        {
            ActionComplete();
        }
    }

    private void OnActionConfirm(object sender, EventArgs eventArgs)
    {
        if (state == State.Aiming)
        {
            unit.SpendActionPoints(GetActionPointCost());
            state = State.AimingConfirmed;
        }
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition gridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(gridPosition);
    }
    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //Grid position is empty, no unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    //both units on same team
                    continue;
                }
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.transform.position - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight, shootDir,
                    Vector3.Distance(targetUnit.transform.position, unitWorldPosition),
                    obstaclesLayerMask))
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
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(position);
        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;
        canShootBullet = true;
        ActionStart(onActionComplete);
    }

    private void Update()
    {
        if (!isActive)
            return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                float aimTurnSpeed = 10f;
                Vector3 aimDirection = (targetUnit.transform.position - transform.position).normalized;
                transform.forward = Vector3.SlerpUnclamped(transform.forward, aimDirection, Time.deltaTime * aimTurnSpeed);
                break;
            case State.Shooting:
                if(canShootBullet)
                {
                    Shoot(DidShooterHit());
                    canShootBullet = false;
                }
                break;

            case State.Cooloff:
                break;
        }

        if(stateTimer <=0f)
        {
            NextState();
        }
    }


    private bool DidShooterHit()
    {
        //TODO: expand aim math
        return UnityEngine.Random.value > GetHitPercentage();
    }

    public float GetHitPercentage()
    {
        return 0.5f;
    }
    private void Shoot(bool hit)
    {
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit,
            hit = hit,
        });
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit,
            hit = hit,
        });
        //TODO: add weapon dependent damage amount
        int damageAmount = hit? 40 : 0;
        targetUnit.Damage(damageAmount);
    }
    private void NextState()
    {
        switch (state)
        {
            case State.AimingConfirmed:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;

            case State.Shooting:
                state = State.Cooloff;
                float cooloffStateTime = 0.5f;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100  + Mathf.RoundToInt((1- targetUnit.GetHealthNormalized())*100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
