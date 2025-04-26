using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private float rotationLimitUp = 80f;
    [SerializeField] private float rotationLimitDown = 60f;

    private float verticalRotation = 0f;

    void Update()
    {
        ManageYRotation();
    }

    private void ManageYRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -rotationLimitDown, rotationLimitUp);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
