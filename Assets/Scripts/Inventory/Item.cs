using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemType itemType;
    [SerializeField]
    private int quantity;
    [SerializeField]
    private Sprite sprite;


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

    public ItemType GetItemType() { return itemType; }
    public int GetQuantity() { return quantity; }
    public Sprite GetSprite() { return sprite; }
}
