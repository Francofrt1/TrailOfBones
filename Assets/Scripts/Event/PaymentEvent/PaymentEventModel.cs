using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaymentEventModel : MonoBehaviour
{
    private int bonesStorage = 0;
    public const int bonesToPay = 2;
    public float interactionDistance = 3.5f;

    public void setBones(int amount)
    {
        bonesStorage += amount;
    }
    public int GetBonesStorage()
    {
        return bonesStorage;
    }
    public int GetBonesToPay()
    {
        return bonesToPay;
    }
    public void UseAllBones()
    {
        bonesStorage = 0;
    }

    public int BonesNeededToPay()
    {
        return bonesToPay - bonesStorage;
    }
}
