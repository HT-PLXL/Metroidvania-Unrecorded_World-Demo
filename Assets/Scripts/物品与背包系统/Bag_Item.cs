using System;

[Serializable]
public class Bag_Item
{
    public ItemDataSO itemData;
    public int StackCount = 1;
    public ItemModifier[] modifiers { get; private set; }
    public Bag_Item(ItemDataSO itemData)
    {
        this.itemData = itemData;
        modifiers = GetEquipmentData()?.modifier;//在判断物品是装备后创建新修饰符列表保存传入的物品的修饰符
    }
    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var modifier in modifiers)
        {
            Stat statNeedModify = playerStats.GetStatByType(modifier.statType);
            statNeedModify.AddModifier(modifier.value, itemData.itemName);
        }
    }
    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var modifier in modifiers)
        {
            Stat statNeedModify = playerStats.GetStatByType(modifier.statType);
            statNeedModify.RemoveModifier(itemData.itemName);
        }
    }
    private EquipmentDataSO GetEquipmentData()
    {
        if (itemData is EquipmentDataSO equipment)//检查itemData是否是EquipmentDataSO类型，如果是赋值给equipment
            return equipment;
        return null;
    }
    public bool CanAddStackCount() => StackCount < itemData.maxStackCount;
    public void AddStackCount() => StackCount++;
    public void ReduceStackCount() => StackCount--;
}
