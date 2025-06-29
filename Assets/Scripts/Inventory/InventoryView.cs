using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : NetworkBehaviour
{
    private List<GameObject> slots = new List<GameObject>();
    private GameObject container;
    void Awake()
    {
        PlayerPresenter.OnPlayerSpawned += HandlePlayerSpawned;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }
    }

    private void HandlePlayerSpawned(PlayerPresenter playerPresenter = null)
    {
        var hudView = GameObject.FindObjectOfType<HUDView>(true);
        if (hudView == null)
        {
            Debug.LogError("HUDView not found!");
            return;
        }

        container = hudView.GetInvetoryMenu();
        if (container != null)
        {
            slots.Clear();
            for (int i = 0; i < container.transform.childCount; i++)
            {
                slots.Add(container.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            Debug.LogError("Inventory container not found!");
        }
    }

    public void UpdateSlot(Dictionary<ItemType, (int Quantity, Sprite Sprite)> inventory)
    {
        slots.ForEach( slot => 
        {
            Image img = slot.GetComponentInChildren<Image>();
            Text amountText = slot.GetComponentInChildren<Text>();

            img.sprite = null;
            amountText.text = "";
        });

        foreach (var item in inventory)
        {
            int itemAmount = item.Value.Quantity;
            Sprite itemSprite = item.Value.Sprite;
            GameObject slot = slots.FirstOrDefault(obj => obj.GetComponentInChildren<Image>().sprite == null);

            if(slot != null)
            {
                Image slotImage = slot.GetComponentInChildren<Image>();
                Text slotAmount = slot.GetComponentInChildren<Text>();
                slotImage.sprite = itemSprite;
                slotAmount.text = itemAmount.ToString();
            }
        }
    }
}
