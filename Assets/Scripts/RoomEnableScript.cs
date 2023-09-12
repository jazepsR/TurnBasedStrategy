using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnableScript : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToEnable;
    [SerializeField] private GameObject fogOfWar;
    [SerializeField] private GameObject interactableRoomEnableObject;
    // Start is called before the first frame update
    void Start()
    {
        ToggleRoomVisibility(false);
        interactableRoomEnableObject.GetComponent<IInteractable>().OnInteract += InteractionComplete;
    }

    private void InteractionComplete(object sender, EventArgs e)
    {
        ToggleRoomVisibility(true);
    }

    private void ToggleRoomVisibility(bool setVisible)
    {
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(setVisible);
            }
        }
        fogOfWar.SetActive(!setVisible);
    }
   
}
