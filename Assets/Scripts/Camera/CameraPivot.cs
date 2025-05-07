using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private float pitchLimitUp = 80f;
    [SerializeField] private float pitchLimitDown = -12f;

    private Transform player;

    private InputHandler inputHandler;

    private float currentPitch = 0f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        inputHandler = player.GetComponent<InputHandler>();
    }

    private void OnEnable()
    {
        inputHandler.OnMouseMoveY += ManagePitch;
    }

    private void OnDisable()
    {
        inputHandler.OnMouseMoveY -= ManagePitch;
    }

    private void ManagePitch(float amount)
    {
        if (Time.timeScale == 0f) return;

        currentPitch -= amount;

        currentPitch = Mathf.Clamp(currentPitch, pitchLimitDown, pitchLimitUp);

        transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }
}
