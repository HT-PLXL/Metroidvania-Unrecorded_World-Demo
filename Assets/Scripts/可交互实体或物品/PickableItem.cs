using System.Collections;
using UnityEngine;
//可拾取物（掉落物）脚本
public class PickableItem : MonoBehaviour
{
    [SerializeField] private ItemDataSO itemData;//需要unity拖入数据
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private Vector2 dropForce = new Vector2(3, 8);
    private bool isPendingLand = false;
    private float pendingTimer = 0f;
    private void Update()
    {
        if (!isPendingLand) return;

        pendingTimer += Time.deltaTime;
        // 条件：速度平缓 或 等待超过 0.5 秒
        bool speedOk = Mathf.Abs(rb.linearVelocity.y) < 0.5f;
        if (speedOk || pendingTimer > 0.5f)
            Land();
    }
    private void Land()
    {
        if (col.isTrigger) return;
        isPendingLand = false;
        col.isTrigger = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    private void OnValidate()
    {
        if (itemData == null)
            return;
        sr = GetComponent<SpriteRenderer>();
        SetupObject();
    }
    public void SetupItem(ItemDataSO itemData)
    {
        this.itemData = itemData;
        SetupObject();

        float xForce = Random.Range(-dropForce.x, dropForce.x);//左右随机
        rb.linearVelocity = new Vector2(xForce, dropForce.y);
        col.isTrigger = false;
    }
    private void SetupObject()
    {
        sr.sprite = itemData.icon;
        gameObject.name = itemData.itemName;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (col.isTrigger) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("地面"))
        {
            if (!isPendingLand)
            {
                isPendingLand = true;
                pendingTimer = 0f;
            }
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)//碰撞触发
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("地面") && col.isTrigger == false)
    //    {
    //        col.isTrigger = true;//变为触发器就不会再执行碰撞方法了，可触发下面方法拾取
    //        rb.constraints = RigidbodyConstraints2D.FreezeAll;//冻结x y z移动
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision)//拾取
    {
        Bag_Player bag = collision.GetComponent<Bag_Player>();
        if (bag == null)
            return;
        Bag_Item itemNeedAdd = new Bag_Item(itemData);
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
