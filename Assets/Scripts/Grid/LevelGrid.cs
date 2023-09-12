using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelGrid : MonoBehaviour
{
    GridSystem<GridObject> grid;
    [SerializeField] private Transform gridDebugObjectPrefab;
    public event EventHandler OnAnyUnitMovedGridPosition;
    public static LevelGrid Instance { get; private set; }

    [SerializeField] private int width= 10, height=10;
    [SerializeField] private float cellSize=2;

    void Awake()
    {
        Instance = this;
        grid = new GridSystem<GridObject>(width, height, cellSize,
            (GridSystem<GridObject> g, GridPosition gridPosition)=> new GridObject(g, gridPosition) );
        //grid.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }

    public void AddUnitAtGridPosition(GridPosition position, Unit unit)
    {
        grid.GetGridObject(position).AddUnit(unit); 
    }

    public List<Unit> GetUnitListAtGridPositio(GridPosition position)
    {
       return grid.GetGridObject(position).GetUnitList();

    }

    public void RemoveUnitAtGridPosition(GridPosition position, Unit unit)
    {
        grid.GetGridObject(position).Remove(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)=>grid.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => grid.GetWorldPosition(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => grid.IsValidGridPosition(gridPosition);

    public int GetWidth() => grid.GetWidth();  

    public int GetHeight() => grid.GetHeight();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        return grid.GetGridObject(gridPosition).HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        return grid.GetGridObject(gridPosition).GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = grid.GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }

    public void SetInteractableAtGridPosition(IInteractable interactable, GridPosition gridPosition)
    {
        GridObject gridObject = grid.GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }
}
