using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryModel : MonoBehaviour
{
    private Dictionary<ItemType, (int quantity,Sprite sprite)> inventory = new Dictionary<ItemType, (int, Sprite)>();

    private const int LIMITITEMAMOUNT = 10;
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

    public void reduceItem(ItemType itemType, int amountToUse)
    {
        if (!inventory.ContainsKey(itemType)) return;
        int quantityTotal = Mathf.Max((inventory[itemType].quantity - amountToUse), 0);
        inventory[itemType] = (quantityTotal, inventory[itemType].sprite);

    }

    public bool checkItemAmount(ItemType itemType, int amountToUse)
    {
        if (!inventory.ContainsKey(itemType)) return false;
        return inventory[itemType].quantity >= amountToUse;
    }

    public Dictionary<ItemType, (int, Sprite)> GetInventory() {  return inventory; }
}
