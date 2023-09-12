using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float timer;
    private enum State
    {
        WaitingForTurn,
        TakingTurn,
        Busy,
    }
    private State state;

    private void Awake()
    {
        state = State.WaitingForTurn;
    }
    private void Start()
    {
        TurnSystem.instance.OnNextTurnStarted += TurnSystem_OnNextTurnStarted;
    }
    void Update()
    {
        if (TurnSystem.instance.IsPlayerTurn())
            return;
        switch(state)
        {
            case State.WaitingForTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    state = State.Busy;
                    if(TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }


       
    }

    private void TurnSystem_OnNextTurnStarted(object sender, EventArgs e)
    {
        if(TurnSystem.instance.IsPlayerTurn())
        {

        }
        else
        {
            float waitTime = 2f;
            state = State.TakingTurn;
            timer = waitTime;
        }
    }
    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }
    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        Debug.Log("take enemy AI action");
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if(enemyUnit.gameObject.activeInHierarchy && TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach(BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if(!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                continue;
            }
            if(bestEnemyAIAction == null) 
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }


        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }            
    }
}
