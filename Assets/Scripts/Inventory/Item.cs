using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemObject item;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(!collision.gameObject.GetComponent<PlayerPresenter>().canBeSaved(this)) return;
            collision.gameObject.GetComponent<PlayerPresenter>().saveItem(this);
            Destroy(gameObject);
        }
    }

    public ItemType GetItemType() { return item.type; }
    public Sprite GetSprite() { return item.sprite; }
    public int GetQuantity() { return item.quantity; }
    public ItemObject GetItem() { return item; }
}
