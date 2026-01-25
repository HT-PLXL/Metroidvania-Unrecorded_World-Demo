using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipSlot : UI_ItemSolt
{
    public ItemType slotType;
    private void OnValidate()
    {
        gameObject.name = "装备-"+ slotType.ToString();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(itemInSlot == null)
            return;
        bag.RemoveEquipItem(itemInSlot);//卸下装备
    }
}
