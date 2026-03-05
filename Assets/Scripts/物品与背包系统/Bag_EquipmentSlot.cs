using System;
using UnityEngine;

[Serializable]
public class Bag_EquipmentSlot
{
    public ItemType slotType;//槽位种类
    public Bag_Item equipmentItem;//装备的物品
    public bool HasItem() => equipmentItem != null && equipmentItem.itemData != null;
}
