using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipSlot : UI_ItemSlot
{
    public ItemType slotType;
    private void OnValidate()
    {
        gameObject.name = "装备-"+ slotType.ToString();
    }
    public override void OnPointerDown(PointerEventData eventData)//继承父类的接口赋予的方法，确定玩家点击背包的物品槽实例
    {
        if(itemInSlot == null)
            return;
        bag.RemoveEquipItem(itemInSlot);//卸下装备
    }
}
