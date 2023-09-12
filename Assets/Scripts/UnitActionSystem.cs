using System;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler OnActionStarted;
    public event EventHandler<bool> OnBusyChanged;
    [SerializeField]private LayerMask unitLyerMask;
    [SerializeField] private Unit selectedUnit;
    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is more than one UnitActionSystem! +" + transform.name);
            Destroy(gameObject);
        }

    }
    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }
    void Update()
    {
        if (isBusy)
        {
            return;
        }
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!TurnSystem.instance.IsPlayerTurn())
        {
            return;
        }
        if (TryHandleUnitSelection())
        {
            return;
        }
        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if(InputManager.instance.IsMouseButtonDown())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                return;
            }

            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            SetBusy();
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }
    /*private void HandleSelectedAction()
    {
        switch(selectedAction)
        {
            case MoveAction moveAction:
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
                if (moveAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    moveAction.Move(mouseGridPosition, ClearBusy);
                    SetBusy();
                }
                break;
            case SpinAction spinAction:
                selectedUnit.GetSpinAction().Spin(ClearBusy);
                SetBusy();
                break;
        }
    }*/
    private void SetBusy()
    {
        isBusy =true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy() 
    {
        isBusy =false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.instance.IsMouseButtonDown())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.GetMouseScreenPosition());
            Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLyerMask);
            if(hit.collider != null && hit.collider.gameObject.TryGetComponent(out Unit unit))
            {
                if (unit == selectedUnit)
                    return false;
                if (unit.IsEnemy())
                    return false;
                SetSelectedUnit(unit);
                return true;
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        SetSelectedAction(unit.GetAction<MoveAction>());
    }

    public void SetSelectedAction(BaseAction action)
    {
        selectedAction = action;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
