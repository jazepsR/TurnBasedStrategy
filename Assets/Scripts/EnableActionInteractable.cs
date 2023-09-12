using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableActionInteractable : MonoBehaviour, IInteractable
{
    public event EventHandler OnInteract;
    private GridPosition gridPosition;
    private float timer;
    private bool isActive =false;
    private Action onInteractComplete;
    public void Interact(Action onInteractionComplete)
    {
        foreach(Unit unit in  FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if(!unit.IsEnemy())
            {
                //TODO: add other interactions
                unit.GetAction<GrenadeAction>().isSkillAvailable = true;
            }
        }

        timer = 0.5f;
        isActive = true;    
        onInteractComplete = onInteractionComplete;
    }

    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(this, gridPosition);
        Pathfinding.Instance.SetWalkableGridPosition(gridPosition, false);

    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isActive = false;
            onInteractComplete();
            OnInteract?.Invoke(this, EventArgs.Empty);
            LevelGrid.Instance.SetInteractableAtGridPosition(null, gridPosition);
            Pathfinding.Instance.SetWalkableGridPosition(gridPosition, true);
           // UnitActionSystem.Instance.SetSelectedAction(UnitActionSystem.Instance.GetSelectedUnit().GetAction<GrenadeAction>());
            Destroy(gameObject);
        }
    }
}
