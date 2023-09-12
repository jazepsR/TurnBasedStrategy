using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShootAction;

public interface IInteractable
{
    public event EventHandler OnInteract;
    void Interact(Action onInteractionComplete);

}
