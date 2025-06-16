using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private float pitchLimitUp = 80f;
    [SerializeField] private float pitchLimitDown = -12f;

    private InputHandler inputHandler;

    private float currentPitch = 0f;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        inputHandler.OnMouseMoveY -= ManagePitch;
    }

    public void SetInputHandler(InputHandler playerInputHandler)
    {
        inputHandler = playerInputHandler;
        inputHandler.OnMouseMoveY += ManagePitch;
    }

    private void ManagePitch(float amount)
    {
        if (Time.timeScale == 0f) return;

        currentPitch -= amount;

        currentPitch = Mathf.Clamp(currentPitch, pitchLimitDown, pitchLimitUp);

        transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }
}
