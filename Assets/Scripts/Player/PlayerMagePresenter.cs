using UnityEngine;

public class PlayerMagePresenter : PlayerPresenter
{
    private ProjectilePoolManager poolManager;
    [SerializeField] private LayerMask projectileLayer;
    private LayerMask resolvedCollisionMask;
    
    protected override void Start()
    {
        base.Start();
        poolManager = GetComponent<ProjectilePoolManager>();

        //This collision mask has the interaction of projectileLayer with the other LayerMask:
        resolvedCollisionMask = LayerCollisionUtils.GetCollisionMaskForLayer(projectileLayer);
    }

    public override void DoAttack()
    {
        Vector3 offset = transform.up * 1f + transform.forward * 2f;
        Vector3 origin = transform.position + offset;

        Vector3? destiny = cameraPivot.GetRaycastHitPoint(100f, resolvedCollisionMask);

        Vector3 direction = (destiny ?? (origin + cameraPivot.Forward * 100f)) - origin;
        Quaternion rotation = Quaternion.LookRotation(direction);

        poolManager.GetBullet(origin, rotation, playerModel.ID);
    }
}