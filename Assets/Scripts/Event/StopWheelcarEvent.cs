using System;
using UnityEngine;

public class StopWheelcarEvent : MonoBehaviour
{
    public event Action<WheelcartController> OnWheelcartToPlay;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DefendableObject"))
        {
            WheelcartController wheelcart = other.GetComponent<WheelcartController>();
            if (wheelcart != null)
            {
                wheelcart.StopPlayWheelcar(true);
                wheelcart.SetStatusInEvent(true);
                OnWheelcartToPlay?.Invoke(wheelcart);
                gameObject.SetActive(false);
            }
        }
    }
}
