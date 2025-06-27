using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using FishNet.Object;
using UnityEngine;

public class ProjectilePoolManager : NetworkBehaviour
{

    [SerializeField] private ObjectPool projectilePool;

    public void GetBullet(Vector3 position, Quaternion rotation, string shooterID)
    {
        GameObject projectile = projectilePool.GetObject();
        projectile.GetComponent<IShootable>().Shoot(shooterID);

        if (projectile != null)
        {
            projectile.transform.position = position;
            projectile.transform.rotation = rotation;
        }
    }
}
