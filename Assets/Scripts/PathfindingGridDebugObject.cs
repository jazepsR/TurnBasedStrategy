using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathfindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TMP_Text gCostText;
    [SerializeField] private TMP_Text hCostText;
    [SerializeField] private TMP_Text fCostText;
    [SerializeField] private SpriteRenderer walkableIndicator;

    private PathNode pathNode;

    public override void SetGridObject(object gridObject)
    {
        pathNode = (PathNode)gridObject;
        base.SetGridObject(gridObject);
    }

    protected override void Update()
    {
        base.Update();
        gCostText.text = pathNode.GetGCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        walkableIndicator.color = pathNode.IsWalkable()? Color.green : Color.red;
    }
}
