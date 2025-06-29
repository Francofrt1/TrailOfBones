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
                OnWheelcartToPlay?.Invoke(wheelcart);
                gameObject.SetActive(false);
            }
        }
    }
}
