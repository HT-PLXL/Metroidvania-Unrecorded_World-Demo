using System.Collections.Generic;

public class Bag_Player : Bag_Base
{
    private Entity_Stats playerStats;
    public List<Bag_EquipmentSlot> equipmentList;//装备槽列表，由unity面板添加
    protected override void Awake()
    {
        base.Awake();
        playerStats = GetComponent<Entity_Stats>();
    }
    public void TryEquipItem(Bag_Item clickedItem)
    {
        Bag_Item item = SearchItem(clickedItem.itemData);
        List<Bag_EquipmentSlot> slots = equipmentList.FindAll(slot => slot.slotType == clickedItem.itemData.itemType);//判断并找出所有装备槽列表里与想要装备的物品相同种类的
        foreach (var slot in slots)//遍历所有同类装备槽
        {
            if (slot.HasItem() == false)//判断是否有空槽位可用
            {
                EquipItem(item, slot);
                return;
            }
        }
    }
    private void EquipItem(Bag_Item itemNeedEquip, Bag_EquipmentSlot slot)
    {
        slot.equipmentItem = itemNeedEquip;
        slot.equipmentItem.AddModifiers(playerStats);
        RemoveItem(itemNeedEquip);//从已有物品列表移除刚刚装备的物品
    }
}
