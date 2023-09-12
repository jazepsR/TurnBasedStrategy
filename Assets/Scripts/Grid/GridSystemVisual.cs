using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Green,
        Yellow,
    }

    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterials;// = new List<GridVisualTypeMaterial>();
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    private GridSystemVisualSingle[,] gridSystemVisualArray;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        gridSystemVisualArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for(int x = 0;x<LevelGrid.Instance.GetWidth();x++)
        {
            for(int z = 0; z<LevelGrid.Instance.GetHeight();z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Transform gridVisualSingleTransform= Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualArray[x,z] = gridVisualSingleTransform.gameObject.GetComponent<GridSystemVisualSingle>();
            }
        }
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        HideAllGridPositions();
    }


    public void HideAllGridPositions()
    {
        for (int x = 0; x < gridSystemVisualArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridSystemVisualArray.GetLength(1); z++)
            {
                gridSystemVisualArray[x,z].Hide();
            }
        }
    }

    public void ShowGridListOfGridPositions(List<GridPosition> gridPositionsToShow, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionsToShow)
        {
            gridSystemVisualArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (selectedAction != null)
        {
            var moveList = selectedAction.GetValidActionGridPositionList();
            HideAllGridPositions();
            GridVisualType gridVisualType = GridVisualType.White;
            switch(selectedAction)
            {
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case SpinAction spinAction:
                    gridVisualType = GridVisualType.Blue;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                    break;
                case GrenadeAction grenadeAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                case SwordAction swordAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                    break;
                case InteractAction interactAction:
                    gridVisualType = GridVisualType.Blue;
                    break;


            }
            ShowGridListOfGridPositions(moveList, gridVisualType);
        }
    }
    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterials)
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.LogError("Could not find GridVisualTypeMaterial for gridVisualType: " + gridVisualType);
        return null;
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();    
        for(int x = -range; x < range; x++) 
        { 
            for(int z =- range; z < range; z++)
            {
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                gridPositionList.Add(testGridPosition);
            }        
        }
        ShowGridListOfGridPositions(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x < range; x++)
        {
            for (int z = -range; z < range; z++)
            {               
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridListOfGridPositions(gridPositionList, gridVisualType);
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
}
