using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnSystem : MonoBehaviour
{
    private int currentTurn = 1;

    public event EventHandler OnNextTurnStarted;
    public static TurnSystem instance { get; private set; }
    private bool isPlayerTurn = true;
    private void Awake()
    {
        instance = this; 
    }
    public void NextTurn() 
    { 
        currentTurn++;
        isPlayerTurn = !isPlayerTurn;
        OnNextTurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public int GetCurrentTurn()
    {
        return currentTurn;
    }

    public bool IsPlayerTurn()
    { 
        return isPlayerTurn; 
    }
}
