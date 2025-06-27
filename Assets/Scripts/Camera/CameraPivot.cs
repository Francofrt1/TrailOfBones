using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private float pitchLimitUp = 80f;
    [SerializeField] private float pitchLimitDown = -12f;

    [SerializeField] private GameObject cameraObject;
    private Camera cameraReference;

    private InputHandler inputHandler;

    private float currentPitch = 0f;

    private void Awake()
    {
        cameraReference = cameraObject.GetComponent<Camera>();
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

    public Vector3? GetRaycastHitPoint(float maxDistance, LayerMask collisionMask)
    {
        Ray ray = cameraReference.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 2f);

        return Physics.Raycast(ray, out RaycastHit hit, maxDistance, collisionMask)
            ? hit.point
            : (Vector3?)null;
    }

    public Vector3 Forward => cameraReference.transform.forward;
}
