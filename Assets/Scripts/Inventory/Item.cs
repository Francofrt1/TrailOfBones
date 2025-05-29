using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemObject item;
    private InventoryController inventoryController;

    private void Start()
    {
        inventoryController = InventoryController.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            inventoryController.HandleAddItem(this);
            Destroy(gameObject);
        }
    }

    public ItemType GetItemType() { return item.type; }
    public Sprite GetSprite() { return item.sprite; }
    public int GetQuantity() { return item.quantity; }
    public ItemObject GetItem() { return item; }
}
