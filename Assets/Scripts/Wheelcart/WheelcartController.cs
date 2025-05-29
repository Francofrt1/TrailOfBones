using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(WheelcartModel))]
[RequireComponent(typeof(WheelcartMovement))]
public class WheelcartController : MonoBehaviour, IDamageable, IDeath, IHealthVariation, IWheelcartDuration
{
    private WheelcartModel wheelcartModel;
    private WheelcartMovement wheelcartMovement;

    public event Action OnDie;
    public event Action<float, float> OnHealthVariation;
    public event Action<float> OnWheelcartDuration;


    private void Awake()
    {
        wheelcartModel = GetComponent<WheelcartModel>();
        wheelcartMovement = GetComponent<WheelcartMovement>();
    }

    private void Start()
    {
        OnWheelcartDuration?.Invoke(wheelcartMovement.GetDuration());
        OnHealthVariation?.Invoke(wheelcartModel.currentHealth, wheelcartModel.maxHealth);
    }

    public void TakeDamage(float damageAmout, string hittedById)
    {
        wheelcartModel.SetHealth(wheelcartModel.currentHealth - damageAmout);

        if(wheelcartModel.currentHealth <= wheelcartModel.StopWheelcartPercent())
        {
            //splineAnimate.Pause();
        }

        if (wheelcartModel.currentHealth <= 0)
        {
            OnDeath(hittedById);
        }
        //Debug.Log(wheelcartModel.currentHealth);
        OnHealthVariation?.Invoke(wheelcartModel.currentHealth, wheelcartModel.maxHealth);
    }

    public void OnDeath(string killedById)
    {
        OnDie?.Invoke();
    }

    public string GetTag()
    {
        return gameObject.tag;
    }

    //private void Update()
    //{
    //    if (!splineAnimate.isPlaying)
    //    {
    //        if (Input.GetKeyDown(KeyCode.T) && InventoryController.Instance.CanUse(ItemType.WoodLog, 2))
    //        {
    //            InventoryController.Instance.HandleUseItem(ItemType.WoodLog,2);
    //            wheelcartModel.SetHealth((int)wheelcartModel.maxHealth);
    //            splineAnimate.Play();
    //        }
    //    }
    //}
}
