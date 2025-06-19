using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : NetworkBehaviour
{
    private Dictionary<ItemType, (int quantity,Sprite sprite)> inventory = new Dictionary<ItemType, (int, Sprite)>();

    private const int LIMITITEMAMOUNT = 99;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }
    }
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

    public bool limitReached(Item item)
    {
        return !inventory.ContainsKey(item.GetItemType()) || inventory[item.GetItemType()].quantity < LIMITITEMAMOUNT;
    }

    public void reduceItem(ItemType itemType, int amountToUse)
    {
        if (!inventory.ContainsKey(itemType)) return;
        int quantityTotal = Mathf.Max((inventory[itemType].quantity - amountToUse), 0);

        if (quantityTotal > 0)
        {
            inventory[itemType] = (quantityTotal, inventory[itemType].sprite);
        }
        else
        {
            inventory.Remove(itemType);
        }
    }

    public bool checkItemAmount(ItemType itemType, int amountToUse)
    {
        if (!inventory.ContainsKey(itemType)) return false;
        return inventory[itemType].quantity >= amountToUse;
    }

    public Dictionary<ItemType, (int, Sprite)> GetInventory() {  return inventory; }
}
