using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaymentEvent : MonoBehaviour
{
    private WheelcartController wheelcart;
    private StopWheelcarEvent stopWheelcarEvent;
    private PaymentEventModel model;

    public event Action<int> OnSetMaxBonesStorageUI;
    public event Action<int> OnChangedBonesStorage;
    public event Action OnPaymentCompleted;

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
        wheelcart = wheelcartController;
    }

    public void StorageBones(int amount)
    {
        model.setBones(amount);
        OnChangedBonesStorage?.Invoke(model.GetBonesStorage());

        if (model.GetBonesStorage() >= model.GetBonesToPay())
        {
            CompletePayment();
        }
    }

    public int NeededBonesToPay()
    {
        return model.BonesNeededToPay();
    }

    public void CompletePayment()
    {
        model.UseAllBones();
        OnPaymentCompleted?.Invoke();
        wheelcart.StopPlayWheelcar(false);
    }
}
