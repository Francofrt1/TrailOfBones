using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopWheelcarEvent : MonoBehaviour
{
    public event Action<WheelcartController> Wheelcart;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DefendableObject"))
        {
            WheelcartController wheelcart = other.GetComponent<WheelcartController>();
            if (wheelcart != null)
            {
                wheelcart.StopPlayWheelcar(true);
                Wheelcart?.Invoke(wheelcart);
                gameObject.SetActive(false);
            }
        }
    }
}
