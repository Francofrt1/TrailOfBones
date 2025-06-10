using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemObject item;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().saveItem(this);
            Destroy(gameObject);
        }
    }

    public ItemType GetItemType() { return item.type; }
    public Sprite GetSprite() { return item.sprite; }
    public int GetQuantity() { return item.quantity; }
    public ItemObject GetItem() { return item; }
}
