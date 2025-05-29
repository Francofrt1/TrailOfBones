using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public ItemType type;
    public Sprite sprite;
    public int quantity;

    
}
