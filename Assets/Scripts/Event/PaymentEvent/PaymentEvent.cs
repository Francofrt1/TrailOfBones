using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Steamworks.InventoryItem;

public class PaymentEvent : MonoBehaviour, IUseInventory
{
    private StopWheelcarEvent stopWheelcarEvent;
    private PaymentEventModel model;

    public event Action<int> OnSetMaxBonesStorageUI;
    public event Action<int> OnChangedBonesStorage;
    [SerializeField]
    private GameObject flamesWall;
   
    public Action<bool> OnPaymentActive;


    private void Awake()
    {
        stopWheelcarEvent = GetComponentInChildren<StopWheelcarEvent>();
        model = GetComponent<PaymentEventModel>();
    }

    private void OnEnable()
    {
        if(stopWheelcarEvent != null)
        {
            stopWheelcarEvent.OnWheelcartToPlay += OnWheelcartFound;
        }
    }

    private void OnDisable()
    {
        if(stopWheelcarEvent != null)
        {
            stopWheelcarEvent.OnWheelcartToPlay -= OnWheelcartFound;
        }
    }

    private void Start()
    {
        OnSetMaxBonesStorageUI?.Invoke(model.BonesNeededToPay());
    }

    private void OnWheelcartFound(WheelcartController wheelcartController)
    {

        wheelcartController.SetPaymentEvent(this);
    }
    public void CompletePayment()
    {
        model.UseAllBones();
        Destroy(flamesWall);
        OnPaymentActive?.Invoke(false);
    }

    public bool CanInteract(Vector3 playerPosition)
    {
        Vector3 couldron = transform.GetChild(0).position;
        return Vector3.Distance(playerPosition, couldron) < model.interactionDistance;
    }

    public int NeededToMake()
    {
        return model.BonesNeededToPay();
    }

    public void StorageItem(int itemAmount)
    {
        model.setBones(itemAmount);
        OnChangedBonesStorage?.Invoke(model.GetBonesStorage());

        if (model.GetBonesStorage() >= model.GetBonesToPay())
        {
            CompletePayment();
        }
    }

    public ItemType ItemTypeNeeded()
    {
        return ItemType.Bone;
    }

}
