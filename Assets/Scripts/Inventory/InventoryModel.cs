using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryModel : MonoBehaviour
{
    private Dictionary<ItemType, (int quantity,Sprite sprite)> inventory = new Dictionary<ItemType, (int, Sprite)>();

    private const int LIMITITEMAMOUNT = 999;
    public void AddItem(Item item)
    {
        if (!inventory.ContainsKey(item.GetItemType())) 
        {
            inventory.Add(item.GetItemType(), (item.GetQuantity(),item.GetSprite()));
        }
        else
        {

            int quantityTotal = Mathf.Min((item.GetQuantity() + inventory[item.GetItemType()].quantity), LIMITITEMAMOUNT);
            inventory[item.GetItemType()] = (quantityTotal,item.GetSprite());
        }

    }

    public void reduceItem(Item item, int amountToUse)
    {
        if (!inventory.ContainsKey(item.GetItemType())) return;
        int quantityTotal = Mathf.Max((inventory[item.GetItemType()].quantity - amountToUse), 0);
        inventory[item.GetItemType()] = (quantityTotal, item.GetSprite());

    }

    public bool checkItemAmount(Item item, int amountToUse)
    {
        if (!inventory.ContainsKey(item.GetItemType())) return false;
        return inventory[item.GetItemType()].quantity >= amountToUse;
    }

    public Dictionary<ItemType, (int, Sprite)> GetInventory() {  return inventory; }
}
