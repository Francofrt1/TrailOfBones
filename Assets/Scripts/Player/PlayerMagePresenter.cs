using UnityEngine;

public class PlayerMagePresenter : PlayerPresenter
{
    private ProjectilePoolManager poolManager;
    [SerializeField] private LayerMask projectileLayer;

    protected override void Start()
    {
        base.Start();
        poolManager = GetComponent<ProjectilePoolManager>();
    }

    public override void DoAttack()
    {
        Vector3 offset = transform.up * 1f + transform.forward * 2f;
        Vector3 origin = transform.position + offset;

        LayerMask validRaycastMask = LayerCollisionUtils.GetCollisionMaskForLayer(projectileLayer);

        Vector3? destiny = cameraPivot.GetRaycastHitPoint(100f, validRaycastMask);

        Vector3 direction = (destiny ?? (origin + cameraPivot.Forward * 100f)) - origin;
        Quaternion rotation = Quaternion.LookRotation(direction);

        poolManager.GetBullet(origin, rotation, playerModel.ID);
    }
}