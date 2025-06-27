using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public sealed class ControlsView : View
{
   
    [SerializeField] private CanvasGroup canvasGroup;

    public override void Initialize()
    {
        
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        base.Initialize(); 
        Debug.Log("ControlsView: Initialize called.");

       
    }

    
}