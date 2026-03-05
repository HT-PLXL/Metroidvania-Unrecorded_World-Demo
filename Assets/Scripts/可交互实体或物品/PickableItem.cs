using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [SerializeField] private ItemDataSO itemData;//需要unity拖入数据
    private SpriteRenderer sr;
    private void OnValidate()
    {
        if (itemData == null)
            return;
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = itemData.icon;
        gameObject.name = itemData.itemName;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bag_Item itemNeedAdd = new Bag_Item(itemData);
        Bag_Player bag = collision.GetComponent<Bag_Player>();
        Bag_Storage storage = bag.storage;

        if (itemData.itemType == ItemType.材料)//如果是材料直接存进材料仓库
        {
            storage.AddMaterialToStorage(itemNeedAdd);
            Destroy(gameObject);
            return;
        }

        if (bag.CanAddItem(itemNeedAdd))
        {
            bag.AddItem(itemNeedAdd);
            Destroy(gameObject);
        }
    }
}
