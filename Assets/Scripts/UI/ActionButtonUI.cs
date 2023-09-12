using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private Image selectedVisual;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;
    private BaseAction baseAction;
    // Start is called before the first frame update
    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        text.text = baseAction.GetActionName().ToUpper();
        button.onClick.AddListener(() =>
        {
           UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    private void Start()
    {
        //selectedVisual.enabled = false;
    }

    public void UpdateSelectedVisual()
    {
        selectedVisual.enabled = UnitActionSystem.Instance.GetSelectedAction() == baseAction;
    }
}
