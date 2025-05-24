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
        try
        {
            GameObject menu = transform.GetChild(0).gameObject;
            ItemType[] itemType = ((ItemType[])System.Enum.GetValues(typeof(ItemType)));

            for (int i = 0; i < menu.transform.childCount; i++)
            {
                slots.Add(menu.transform.GetChild(i).gameObject);
            }
            /*
            if(slots.Count < itemType.Length) { throw new Exception("Need more slots for new objects"); }

            for (int i = 0; i < itemType.Length; i++)
            {
                slots[i].gameObject.name = itemType[i].ToString();
            }*/
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }


    public void UpdateSlot(int quantity, Sprite sprite)
    {
        GameObject slot = slots.FirstOrDefault(obj => obj.GetComponentInChildren<Image>() == sprite);
        if (slot == null) { 
            if(slots.Any(slot => slot.GetComponentInChildren<Image>().sprite == null)){
                slot = slots.FirstOrDefault(obj => obj.GetComponentInChildren<Image>().sprite == null);
            }
            else
            {
                return;
            } 
        }

        slot.GetComponentInChildren<Text>().text = quantity.ToString();
        slot.GetComponentInChildren<Image>().sprite = sprite;
    }
}
