using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Wood Object", menuName = "Inventory System/Items/Wood")]
public class WoodObject : ItemObject
{
    public void Awake()
    {
        type = ItemType.WoodLog;
    }
}
