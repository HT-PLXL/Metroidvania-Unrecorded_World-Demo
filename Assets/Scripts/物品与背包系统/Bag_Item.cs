using System;

[Serializable]
public class Bag_Item
{
    private string itemId;
    public ItemDataSO itemData;
    public int StackCount = 1;
    public ItemModifier[] modifiers { get; private set; }
    public ItemEffectDataSO itemEffect;
    public Bag_Item(ItemDataSO itemData)
    {
        this.itemData = itemData;
        modifiers = GetEquipmentData()?.modifier;//在判断物品是装备后创建新修饰符列表保存传入的物品的修饰符
        itemEffect = itemData.itemEffect;
        itemId = itemData.itemName + Guid.NewGuid();//生成一个几乎不可能重复的id
    }
    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var modifier in modifiers)
        {
            Stat statNeedModify = playerStats.GetStatByType(modifier.statType);
            statNeedModify.AddModifier(modifier.value, itemId);
        }
    }
    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var modifier in modifiers)
        {
            Stat statNeedModify = playerStats.GetStatByType(modifier.statType);
            statNeedModify.RemoveModifier(itemId);//使用不重复的id作为来源去移除修饰符，确保准确
        }
    }
    public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);//装备新装备时订阅物品效果（如果有）
    public void RemoveItemEffect()=>itemEffect?.Unsubscribe();//卸下新装备时订阅物品效果（如果有）
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
