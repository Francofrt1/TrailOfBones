using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bone Object", menuName = "Inventory System/Items/Bone")]
public class BoneObject : ItemObject
{
    private void Awake()
    {
        type = ItemType.Bone;
    }
}
