using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 2;
    private const float MAX_FOLLOW_Y_OFFSET = 12;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private Vector3 targetZoom = Vector3.zero;
    float zoomLerpSpeed = 20f;
    private CinemachineTransposer transposer;

    private void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }
    void Update()
    {
        HandleTranslation();
        HandleRotation();
        HandleZoom();  
    }

    public void HandleTranslation()
    {
        Vector2 cameraMoveVector = InputManager.instance.GetCameraMoveVector();
        Vector3 moveVector = transform.forward * cameraMoveVector.y + transform.right * cameraMoveVector.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    public void HandleRotation()
    {
       Vector3 rotationVector = new Vector3(0, InputManager.instance.GetRotationValue(), 0);    
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    public void HandleZoom()
    {
        if (InputManager.instance.GetZoomValue() != 0f)
        {
            Vector3 followOffset = transposer.m_FollowOffset;
            followOffset.y = Mathf.Clamp(followOffset.y + InputManager.instance.GetZoomValue() * zoomSpeed,
                MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
            targetZoom = followOffset;
        }
        if (targetZoom != Vector3.zero)
        {
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetZoom, Time.deltaTime * zoomLerpSpeed);
        }
    }
}
