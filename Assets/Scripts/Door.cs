using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static ShootAction;

public class Door : MonoBehaviour, IInteractable
{
    public event EventHandler OnInteract;
    private GridPosition gridPosition;
    private Action onInteractComplete;
    [SerializeField] private bool isOpen;
    private float timer;
    private bool isActive = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(this, gridPosition);
        if (isOpen)
        {
            OpenDoor();
            animator.Play("doorOpen", 0,1);
        }
        else
        {
            CloseDoor();
            animator.Play("doorClose",0,1);
        }
    }
    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = 0.5f;
        Debug.Log("Door.Interact");

        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    private void Update()
    {
        if(!isActive)
        {
            return;
        }
        timer -= Time.deltaTime;
        if(timer <= 0 )
        {
            isActive = false;
            onInteractComplete();
            OnInteract?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("isOpen", true);
        Pathfinding.Instance.SetWalkableGridPosition(gridPosition, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("isOpen", false);
        Pathfinding.Instance.SetWalkableGridPosition(gridPosition, false);
    }
}
