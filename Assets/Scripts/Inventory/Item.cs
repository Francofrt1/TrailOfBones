using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemObject item;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(!collision.gameObject.GetComponent<PlayerPresenter>().CanBeSaved(this)) return;
            collision.gameObject.GetComponent<PlayerPresenter>().SaveItem(this);
            Destroy(gameObject);
        }
    }

    public ItemType GetItemType() { return item.type; }
    public Sprite GetSprite() { return item.sprite; }
    public int GetQuantity() { return item.quantity; }
    public ItemObject GetItem() { return item; }
}
