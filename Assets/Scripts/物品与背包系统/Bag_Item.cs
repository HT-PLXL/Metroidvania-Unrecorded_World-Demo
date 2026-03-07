using System;
using System.Text;
                  //——————物品类——————
[Serializable]
public class Bag_Item
{
    private string itemId;
    public ItemDataSO itemData;
    public int stackCount = 1;
    public ItemModifier[] modifiers { get; private set; }
    public ItemEffectDataSO itemEffect;

    public int buyPrice {  get; private set; }
    public float sellPrice { get; private set; }
    public Bag_Item(ItemDataSO itemData)
    {
        this.itemData = itemData;
        buyPrice = itemData.itemPrice;
        sellPrice = itemData.itemPrice * .5f;
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
    public bool CanAddStackCount() => stackCount < itemData.maxStackCount;
    public void AddStackCount() => stackCount++;
    public void ReduceStackCount() => stackCount--;

    public string GetItemInfo()
    {
        if (itemData.itemType == ItemType.材料)
            return "合成材料";
        if (itemData.itemType == ItemType.消耗品)
            return itemData.itemEffect.effectDescription;//显示消耗品描述
        StringBuilder sb = new StringBuilder();

        foreach (var modifier in modifiers)
        {
            string modifierType = modifier.statType.GetChineseName();
            string modifierValue = IsPercentStat(modifier.statType) ? modifier.value.ToString() + "%" : modifier.value.ToString();

            string sign = modifier.value >= 0 ? "+" : "";//负号应当在物品数据里
            sb.AppendLine(sign + modifierValue + " " + modifierType);
        }
        if (itemEffect != null)
        {
            sb.AppendLine("");
            sb.AppendLine("被动技能：");
            sb.AppendLine(itemEffect.effectDescription);
        }
        return sb.ToString();
    }
    private bool IsPercentStat(StatType type)//判断是否要加%符号
    {
        switch (type)
        {
            case StatType.AttackSpeed:
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.Evasion:
            case StatType.ArmorReduction:
            case StatType.FireResistance:
            case StatType.IceResistance:
            case StatType.LightningResistance:
                return true;
            default:
                return false;
        }
    }
}
