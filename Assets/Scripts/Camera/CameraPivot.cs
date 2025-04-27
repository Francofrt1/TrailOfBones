using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float rotationLimitUp = 80f;
    [SerializeField] private float rotationLimitDown = -15f;

    private Transform player;

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    private void Awake() {
        player = GameObject.Find("Player").transform;
    }
    
    void Update()
    {
        UpdatePosition();
        ManageRotation();
    }

    private void UpdatePosition()
    {
        transform.position = player.transform.position;
    }

    private void ManageRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float mouseInputX = Input.GetAxis("Mouse X") * mouseSensitivity;

        verticalRotation -= mouseY;
        horizontalRotation += mouseInputX;
        verticalRotation = Mathf.Clamp(verticalRotation, rotationLimitDown, rotationLimitUp);

        transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }
}
