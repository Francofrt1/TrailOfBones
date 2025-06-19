using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    private List<GameObject> slots = new List<GameObject>();
    private GameObject container;
    void Awake()
    {
        PlayerPresenter.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(PlayerPresenter playerPresenter)
    {
        container = GameObject.Find("InventoryMenu");

        for (int i = 0; i < container.transform.childCount; i++)
        {
            slots.Add(container.transform.GetChild(i).gameObject);
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
