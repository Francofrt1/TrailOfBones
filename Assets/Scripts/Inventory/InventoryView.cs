using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class InventoryView : MonoBehaviour
{
    private List<GameObject> slots = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GameObject menu = transform.GetChild(0).gameObject;

        for (int i = 0; i < menu.transform.childCount; i++)
        {
            slots.Add(menu.transform.GetChild(i).gameObject);
        }
        
    }


    public void UpdateSlot(int quantity, Sprite sprite)
    {
        //find a space that has the same sprite
        GameObject slot = slots.FirstOrDefault(obj => obj.GetComponentInChildren<Image>().sprite == sprite);
        if (slot == null) {
            //If it doesn't find a space containing the sprite, it looks for an empty one. If there aren't any empty spaces, it exits the method without doing anything.
            if (slots.Any(slot => slot.GetComponentInChildren<Image>().sprite == null)){
                slot = slots.FirstOrDefault(obj => obj.GetComponentInChildren<Image>().sprite == null);
            }
            else
            {
                return;
            } 
        }

        //finds the children that have the text and image components and gives them the new values.
        slot.GetComponentInChildren<Text>().text = quantity.ToString();
        slot.GetComponentInChildren<Image>().sprite = sprite;
    }

    public void FreeUpSlot(Sprite sprite)
    {
        if(slots.Any(slot => slot.GetComponentInChildren<Image>().sprite == sprite)){
            GameObject slot = slots.FirstOrDefault(obj => obj.GetComponentInChildren<Image>().sprite == sprite);
            //finds the children that have the text and image components and gives them the new values.
            slot.GetComponentInChildren<Text>().text = "0";
            slot.GetComponentInChildren<Image>().sprite = null;
        }
    }
}
