using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmActionUI : MonoBehaviour
{
    [SerializeField] TMP_Text successChanceText;
    private CanvasGroup canvasGroup;
    public static event EventHandler OnAnyActionConfirm;
    public static event EventHandler OnAnyActionCancel;

    // Start is called before the first frame update
    void Awake()
    {
        canvasGroup= GetComponent<CanvasGroup>();
        ToggleVisibility(false);
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
    }

    private void BaseAction_OnAnyActionStarted(object sender, System.EventArgs e)
    {
       if(sender is BaseAction && ((BaseAction)sender).needsConfirmation)
        {
            if (TurnSystem.instance.IsPlayerTurn())
            {
                ToggleVisibility(true);
                successChanceText.text = "Success chance: " + ((ShootAction)sender).GetHitPercentage() * 100f + "%";
            }
            else
            {
                ConfirmClick();
            }
        }
    }

    private void ToggleVisibility(bool visible)
    {
        canvasGroup.interactable = visible;
        canvasGroup.alpha = visible ? 1 : 0;
        canvasGroup.blocksRaycasts = visible;
    }
    // Update is called once per frame
    public void CancelClick()
    {
        OnAnyActionCancel?.Invoke(this, EventArgs.Empty);
        ToggleVisibility(false);
    }

    public void ConfirmClick()
    {
        OnAnyActionConfirm?.Invoke(this, EventArgs.Empty);
        ToggleVisibility(false);
    }
}
