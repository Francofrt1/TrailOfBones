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
    private Animation completePaymentAnim;

    private void Awake()
    {
        paymentEvent = GetComponent<PaymentEvent>();
        completePaymentAnim = GetComponentInChildren<Animation>();
    }

    private void OnEnable()
    {
        paymentEvent.OnSetMaxBonesStorageUI += SetBonesToPay;
        paymentEvent.OnChangedBonesStorage += ChangeBonesAmountText;

    }
    private void OnDisable()
    {
        paymentEvent.OnSetMaxBonesStorageUI -= SetBonesToPay;
        paymentEvent.OnChangedBonesStorage -= ChangeBonesAmountText;
    }

    private void SetBonesToPay(int amount)
    {
        bonesToPay = amount.ToString();
        ChangeBonesAmountText(0);
    }

    private void ChangeBonesAmountText(int amount)
    {
        bonesAmountText.text = amount.ToString() + " / " + bonesToPay;
    }
}
