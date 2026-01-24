using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//物品槽脚本，置于物品槽对象上
public class UI_ItemSolt : MonoBehaviour,IPointerDownHandler
{
    public Bag_Item itemInSlot { get; private set; }
    private Bag_Player bag;
    [Header("物品槽设置")]
    [SerializeField] private Image itemIcon;//unity面板选择
    [SerializeField] private TextMeshProUGUI itemStackCount;
    private void Awake()
    {
        bag = FindAnyObjectByType<Bag_Player>();
    }
    public void UpdateSlot(Bag_Item item)//更新具体槽位信息
    {
        itemInSlot = item;//令具体槽位为传入的某物
        if (itemInSlot == null)
        {
            itemStackCount.text = "";//隐藏数量文本
            itemIcon.color = Color.clear;//图标透明
            return;
        }
        Color color = Color.white;
        color.a = 0.9f;
        itemIcon.color = color;
        itemIcon.sprite = itemInSlot.itemData.icon;
        itemStackCount.text = item.StackCount > 1 ? item.StackCount.ToString() : "";
    }

    public void OnPointerDown(PointerEventData eventData)//玩家点击背包的物品
    {
        if(itemInSlot == null)
            return;
        bag.TryEquipItem(itemInSlot);//尝试装备点击的物品
    }
}
