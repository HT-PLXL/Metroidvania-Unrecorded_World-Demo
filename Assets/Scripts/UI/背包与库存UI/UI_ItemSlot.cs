using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//物品槽脚本，置于物品槽对象上
public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Bag_Item itemInSlot { get; private set; }
    protected Bag_Player bag;
    protected UI ui;
    protected RectTransform rect;
    [Header("物品槽设置")]
    [SerializeField] protected Image itemIcon;//unity面板选择
    [SerializeField] protected TextMeshProUGUI itemStackCount;

    protected virtual void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
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
        itemStackCount.text = item.stackCount > 1 ? item.stackCount.ToString() : "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)//接口赋予的方法，确定玩家点击的物品槽实例
    {
        if (itemInSlot == null || itemInSlot.itemData.itemType == ItemType.材料)
            return;
        bool deleteInput = Input.GetKey(KeyCode.LeftControl);
        if (deleteInput)
        {
            bag.itemList.Remove(itemInSlot);//删一组
        }
        else
        {
            if (itemInSlot.itemData.itemType == ItemType.消耗品)
            {
                bag.TryUseItem(itemInSlot);
            }
            else
                bag.TryEquipItem(itemInSlot);//尝试装备点击的物品
        }

        if (itemInSlot == null)//最后一个有装备的格子在装备后会为空，需要再次判断
            ui.itemTooltip.ShowTooltip(false, null);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        ui.itemTooltip.ShowTooltip(true, rect, itemInSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemTooltip.ShowTooltip(false, null);
    }

}
