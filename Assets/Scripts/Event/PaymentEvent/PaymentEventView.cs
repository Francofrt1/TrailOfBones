using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Steamworks.InventoryItem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PaymentEventView : MonoBehaviour
{
    private PaymentEvent paymentEvent;
    public TextMeshProUGUI bonesAmountText;
    private string bonesToPay;

    private void Awake()
    {
        paymentEvent = GetComponent<PaymentEvent>();
    }

    private void OnEnable()
    {
        paymentEvent.OnSetMaxBonesStorageUI += SetBonesToPay;
        paymentEvent.onChangedBonesStorage += ChangeBonesAmountText;

    }
    private void OnDisable()
    {
        paymentEvent.OnSetMaxBonesStorageUI -= SetBonesToPay;
        paymentEvent.onChangedBonesStorage -= ChangeBonesAmountText;
    }

    private void SetBonesToPay(int amount)
    {
        bonesToPay = amount.ToString();
    }

    private void Start()
    {
        ChangeBonesAmountText(0);
    }

    private void ChangeBonesAmountText(int amount)
    {
        bonesAmountText.text = amount.ToString() + " / " + bonesToPay;
    }
}
