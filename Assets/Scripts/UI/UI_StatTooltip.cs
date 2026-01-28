using TMPro;
using UnityEngine;

public class UI_StatTooltip : UI_Tooltip
{
    private Player_Stats playerStsts;
    private TextMeshProUGUI statTooltipText;
    protected override void Awake()
    {
        base.Awake();
        playerStsts = FindFirstObjectByType<Player_Stats>();
        statTooltipText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void ShowTooltip(bool show, RectTransform targetRect, StatType statType)
    {
        base.ShowTooltip(show, targetRect);
        statTooltipText.text = GetStatsTooltip(statType);

    }
    public string GetStatsTooltip(StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength:
                return "每点力量+0.5%的暴击伤害";
            case StatType.Agility:
                return "每点敏捷+0.5%暴击率" + "\n每点敏捷+0.5%闪避率";
            case StatType.Intelligence:
                return "每点智力+2元素伤害" + "\n每点智力+0.5%各元素抗性";
            case StatType.Vitality:
                return "每点活力值+2护甲" + "\n每点活力+5最大生命";
            case StatType.MaxHP:
                return "“血量在蹦极”" + "\n受活力影响";
            case StatType.HpRegen:
                return "由装备/buff提供";
            case StatType.Evasion:
                return "有概率闪避伤害"+"\n闪避率上限85%";
            case StatType.Armor:
                return "护甲值可减免物理伤害，最高99%" + "当前伤害减免为：" + playerStsts.GetArmorMitigation(0) * 100 + "%";
            //"\n最终伤害减免 = 护甲*（1-护甲削减）/（护甲*（1-护甲削减）+100）";
            case StatType.Damage:
                return "受力量影响";
            case StatType.CritChance:
                return "受敏捷影响";
            case StatType.CritPower:
                return "受力量影响";
            case StatType.ArmorReduction:
                return "攻击削减对方物理护甲百分比";
            case StatType.AttackSpeed:
                return "决定普通攻击速度";
            case StatType.FireDamage:
                return "火元素附着会持续灼烧";
            case StatType.IceDamage:
                return "冰元素附着会持续减速";
            case StatType.LightningDamage:
                return "雷元素附着达到上限会引发雷暴，造成大量伤害";
            case StatType.ElementDamage:
                return "元素伤害总和 = 主元素伤害+50%副元素伤害+智力加成";
            case StatType.FireResistance:
                return "火元素抵抗，受智力影响，上限99%";
            case StatType.IceResistance:
                return "冰元素抵抗，受智力影响，上限99%";
            case StatType.LightningResistance:
                return "雷元素抵抗，受智力影响，上限99%";
            default:
                return "没有适合的属性提示";
        }
    }
}
