using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static ShootAction;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    private Action onInteractComplete;
    private bool isGreen = false;
    private float timer;
    private bool isActive = false;
    private GridPosition gridPosition;

    public event EventHandler OnInteract;

    private void Start()
    {
        SetColorGreen();
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
        }
    }



    private void SetColorGreen()
    {
        meshRenderer.material = greenMaterial;
        isGreen = true;
    }

    private void SetcolorRed()
    {
        meshRenderer.material = redMaterial;
        isGreen = false;
    }

    public void Interact(Action onInteractComplete)
    {
        isActive = true;
        this.onInteractComplete = onInteractComplete;
        timer = 0.5f;
        if (isGreen)
            SetcolorRed();
        else
            SetColorGreen();
    }

}
