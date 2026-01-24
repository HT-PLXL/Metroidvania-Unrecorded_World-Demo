using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [SerializeField] private ItemDataSO itemData;//需要unity拖入数据
    private SpriteRenderer sr;
    private Bag_Item itemNeedAdd;
    private Bag_Base bag;
    private void Awake()
    {
        itemNeedAdd = new Bag_Item(itemData);//将数据转化为物品实例
    }
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
        bag = collision.GetComponent<Bag_Base>();
        if (bag != null && bag.CanAddItem())
        {
            bag.AttItem(itemNeedAdd);
            Destroy(gameObject);
        }
    }
}
