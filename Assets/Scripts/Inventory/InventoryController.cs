using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryModel))]
[RequireComponent(typeof(InventoryView))]
public class InventoryController : MonoBehaviour
{
    private InventoryModel model;
    private InventoryView View;
    public static InventoryController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        model = GetComponent<InventoryModel>();
        View = GetComponent<InventoryView>();
    }

    public void HandleAddItem(Item item)
    {
        model.AddItem(item);

        var (quantity, sprite) = model.GetInventory()[item.GetItemType()];

        View.UpdateSlot(quantity, sprite);
    }

    public void HandleUseItem(Item item, int amountToUse) 
    {
        if (!model.checkItemAmount(item, amountToUse)) return;

        //TODO: logic to complete the event

        model.reduceItem(item,amountToUse);

        var (quantity,sprite) = model.GetInventory()[item.GetItemType()];

        View.UpdateSlot(quantity, sprite);

        
    }
}
