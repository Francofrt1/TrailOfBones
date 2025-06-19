using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using static Steamworks.InventoryItem;

[RequireComponent(typeof(WheelcartModel))]
[RequireComponent(typeof(WheelcartMovement))]
public class WheelcartController : MonoBehaviour, IDamageable, IDeath, IHealthVariation, IWheelcartDuration, IUseInventory
{
    private WheelcartModel wheelcartModel;
    private WheelcartMovement wheelcartMovement;

    public event Action OnDie;
    public static event Action<WheelcartController> OnWheelCartSpawned;
    public event Action<float, float> OnHealthVariation;
    public event Action<float> OnWheelcartDuration;
    public event Action<bool> OnBlockWheelcartRequested;
    public event Action<int> OnChangedLogStorage;
    public event Action<int> OnSetMaxLogStorageUI;
    public event Action OnShowLogStorageUI;

    private void Awake()
    {
        wheelcartModel = GetComponent<WheelcartModel>();
        wheelcartMovement = GetComponent<WheelcartMovement>();
    }

    private void Start()
    {
        OnSetMaxLogStorageUI?.Invoke(wheelcartModel.GetLogToRepair());
    }

    public void OnWheelcartSpawned()
    {
        OnWheelcartDuration?.Invoke(wheelcartMovement.GetDuration());
        OnHealthVariation?.Invoke(wheelcartModel.currentHealth, wheelcartModel.maxHealth);
        OnSetMaxLogStorageUI?.Invoke(wheelcartModel.GetLogToRepair());
        OnWheelCartSpawned?.Invoke(this);
    }

    public void TakeDamage(float damageAmout, string hittedById)
    {
        wheelcartModel.SetHealth(wheelcartModel.currentHealth - damageAmout);

        if(NeedRepair())
        {
            StopPlayWheelcar(true);
            OnShowLogStorageUI?.Invoke();
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

    public bool NeedRepair()
    {
        return wheelcartModel.currentHealth <= wheelcartModel.StopWheelcartPercent();
    }

    public void Repair()
    {
        wheelcartModel.UseAllLogs();
        wheelcartModel.SetHealth((int)wheelcartModel.maxHealth);
        OnChangedLogStorage?.Invoke(0);
        OnShowLogStorageUI?.Invoke();
        StopPlayWheelcar(false);
    }

    public void StopPlayWheelcar(bool isPaused)
    {
        OnBlockWheelcartRequested?.Invoke(isPaused);
    }

    public bool CanInteract(Vector3 playerPosition)
    {
        Vector3 wheelcartCenter = transform.GetChild(0).position;
        return Vector3.Distance(playerPosition, wheelcartCenter) < wheelcartModel.interactionDistance && NeedRepair();
    }

    public int NeededToMake()
    {
        return wheelcartModel.LogsNeededToRepair();
    }

    public void StorageItem(int itemAmount)
    {
        wheelcartModel.AddLog(itemAmount);
        OnChangedLogStorage?.Invoke(wheelcartModel.logStorage);

        if (wheelcartModel.logStorage >= WheelcartModel.logToRepair)
        {
            Repair();
        }
    }

    public ItemType ItemTypeNeeded()
    {
        return ItemType.WoodLog;
    }
}
