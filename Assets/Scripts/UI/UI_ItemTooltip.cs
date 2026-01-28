using System.Text;
using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField][cn("物品名")] private TextMeshProUGUI itemName;
    [SerializeField][cn("物品种类")] private TextMeshProUGUI itemType;
    [SerializeField][cn("物品信息")] private TextMeshProUGUI itemInfo;

    public void ShowTooltip(bool show, RectTransform targetRect, Bag_Item itemNeedToShow)
    {
        base.ShowTooltip(show, targetRect);
        itemName.text = itemNeedToShow.itemData.itemName;
        itemType.text = itemNeedToShow.itemData.itemType.ToString();
        itemInfo.text = GetItemInfo(itemNeedToShow);
    }
    public string GetItemInfo(Bag_Item item)//悬浮时物品信息来源
    {
        if (item.itemData.itemType == ItemType.材料)
            return "合成材料";
        if (item.itemData.itemType == ItemType.消耗品)
            return item.itemData.itemEffect.effectDescription;//显示消耗品描述
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("");
        foreach (var modifier in item.modifiers)
        {
            string modifierType = modifier.statType.GetChineseName();
            string modifierValue = IsPercentStat(modifier.statType) ? modifier.value.ToString() + "%" : modifier.value.ToString();

            string sign = modifier.value >= 0 ? "+" : "";//负号应当在物品数据里
            sb.AppendLine(sign + modifierValue + " " + modifierType);
        }
        if(item.itemEffect != null )
        {
            sb.AppendLine("");
            sb.AppendLine("被动技能：");
            sb.AppendLine(item.itemEffect.effectDescription);
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