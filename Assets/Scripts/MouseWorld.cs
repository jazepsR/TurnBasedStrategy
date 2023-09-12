using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] private LayerMask mouseLayerMask;
    private static MouseWorld instance;
    private void Awake()
    {
        instance = this; 
    }
    private void Update()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.Log(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, mouseLayerMask));
        transform.position = GetPosition();
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, instance.mouseLayerMask);
        return hit.point;
    }
}
