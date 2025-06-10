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

    public void HandleAddItem(Item item)
    {
        model.AddItem(item);

        var (quantity, sprite) = model.GetInventory()[item.GetItemType()];

        View.UpdateSlot(quantity, sprite);
    }

    public void HandleUseItem(ItemType itemType, int amountToUse) 
    {
        if (!model.checkItemAmount(itemType, amountToUse)) return;

        //TODO: logic to complete the event

        model.reduceItem(itemType, amountToUse);

        var (quantity,sprite) = model.GetInventory()[itemType];

        View.UpdateSlot(quantity, sprite);

        if (quantity <= 0) View.FreeUpSlot(sprite);
    }

    public bool CanUse(ItemType itemType, int amountToUse)
    {
        return model.checkItemAmount(itemType, amountToUse);
    }
}
