using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.Mathematics;
using UnityEngine;

public class GridSystem<TGridObject> 
{
    private int height, width;
    private float cellSize;
    public TGridObject[,] gridObjectArray;
    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridObjectArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                gridObjectArray[x, z] = createGridObject(this, new GridPosition(x, z));   
                //Debug.DrawLine(GetWorldPosition(x,z), GetWorldPosition(x,z)+ Vector3.right*0.2f, Color.white,1000);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x * cellSize, 0, gridPosition.z * cellSize);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x/cellSize),
            Mathf.RoundToInt(worldPosition.z/cellSize));
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z); 
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), quaternion.identity);
                GridDebugObject gridDebugObject= debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                //Debug.DrawLine(GetWorldPosition(x,z), GetWorldPosition(x,z)+ Vector3.right*0.2f, Color.white,1000);
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];   
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.z >= 0 && 
               gridPosition.x < width &&
               gridPosition.z < height;
    }

   
}


