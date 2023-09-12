using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;
    void Start()
    {
        UpdateActionPointsText();
        Unit.OnAnyActionPointsChanged += OnAnyAction_PointsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        UpdateHealthBar();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void OnAnyAction_PointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e) {
        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }
}
