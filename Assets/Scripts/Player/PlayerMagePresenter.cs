using UnityEngine;

public class PlayerMagePresenter : PlayerPresenter
{
    public override void DoAttack()
    {
        ProjectilePoolManager poolManager = GetComponent<ProjectilePoolManager>();

        // Offset relativo a la orientación del jugador
        Vector3 offset = transform.up * 1f + transform.forward * 2f;

        poolManager.GetBullet(transform.position + offset, transform.rotation, playerModel.ID);
    }
}