using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum Direction { up, down, left, rigtht };
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    private int highCoverDefence = 40;
    private int lowCoverDefence = 20;
    private GridPosition gridPosition;
    private BaseAction[] baseActionArray;
    private int actionPoints = 5;
    [SerializeField] private int maxActionPoints = 5;
    [SerializeField] private bool isEnemy = false;
    private HealthSystem healthSystem;
    private int[] coverValues;
    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
        actionPoints = maxActionPoints;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.instance.OnNextTurnStarted += TurnSystem_OnNextTurnStarted;
        healthSystem.OnDead += HealthSystem_OnDead;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void CalculateCover()
    {
        
    }

    private int GetCoverValue(Vector3 direction)
    {
        if(Physics.Raycast(transform.position, direction, out RaycastHit hit,3f))
        {
            if(hit.transform.tag == "coverHigh")
            {
                return highCoverDefence;
            }
            if (hit.transform.tag == "coverLow")
            {
                return lowCoverDefence;
            }
        }
        return 0;
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach(BaseAction action in baseActionArray)
        {
            if(action is T)
            {
                return (T)action;
            }
        }
        return null;
    }   
    private void Update()
    {       

        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition( this, oldGridPosition, gridPosition);
        }
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction actionToTest)
    {
        return actionPoints >= actionToTest.GetActionPointCost();
    }

    public void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HealthSystem_OnDead(object sender, EventArgs empty)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }
    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if(CanSpendActionPointsToTakeAction(baseAction))
        {
            if(!baseAction.needsConfirmation) //Action points will be spent on confirmation
                SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnNextTurnStarted(object sender, EventArgs empty)
    {
        ResetActionPoints();
    }

    private void ResetActionPoints()
    {
        if ((IsEnemy() && !TurnSystem.instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.instance.IsPlayerTurn()))
        {
            actionPoints = maxActionPoints;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damage)
    {
        healthSystem.Damage(damage);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();  
    }
}
