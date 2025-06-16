using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    private List<GameObject> slots = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GameObject menu = transform.GetChild(0).gameObject;

        for (int i = 0; i < menu.transform.childCount; i++)
        {
            slots.Add(menu.transform.GetChild(i).gameObject);
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
