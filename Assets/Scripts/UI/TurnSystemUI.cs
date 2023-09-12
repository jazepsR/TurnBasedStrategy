using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button nextTurnButton;
    [SerializeField] private TMP_Text currentTurnText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;
    // Start is called before the first frame update
    void Start()
    {
        nextTurnButton.onClick.AddListener(TurnSystem.instance.NextTurn);
        TurnSystem.instance.OnNextTurnStarted += TurnSystem_OnNextTurnStarted;
        UpdateCurrentTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }
        
    private void TurnSystem_OnNextTurnStarted(object sender, EventArgs empty)
    {
        UpdateCurrentTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateCurrentTurnText()
    {
        currentTurnText.text = "TURN " + TurnSystem.instance.GetCurrentTurn();
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        nextTurnButton.gameObject.SetActive(TurnSystem.instance.IsPlayerTurn());
    }
}
