using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryModel))]
[RequireComponent(typeof(InventoryView))]
public class InventoryController : MonoBehaviour
{
    private InventoryModel model;
    private InventoryView View;

    private void Awake()
    {
        model = GetComponent<InventoryModel>();
        View = GetComponent<InventoryView>();
    }

    public bool canBeSaved(Item item)
    {
        return model.limitReached(item);
    }

    public void HandleAddItem(Item item)
    {
        model.AddItem(item);

        View.UpdateSlot(model.GetInventory());
    }

    public void HandleUseItem(ItemType itemType, int amountToUse) 
    {
        if (!model.checkItemAmount(itemType, amountToUse)) return;

        model.reduceItem(itemType, amountToUse);

        View.UpdateSlot(model.GetInventory());
    }

    public int GetItemQuantity(ItemType itemType)
    {
        if (!model.GetInventory().ContainsKey(itemType)) return 0;
        return model.GetInventory()[itemType].Item1;
    }

    public bool CanUse(ItemType itemType, int amountToUse)
    {
        return model.checkItemAmount(itemType, amountToUse);
    }
}
