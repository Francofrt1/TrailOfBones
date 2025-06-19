using System;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IUseInventory
    {
        public bool CanInteract(Vector3 playerPosition);
        public int NeededToMake();
        public void StorageItem(int itemAmount);
        public ItemType ItemTypeNeeded();

    }
}