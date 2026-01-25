using TMPro;
using UnityEngine;

public class UI_StatSlot : MonoBehaviour
{
    private Entity_Stats playerStats;
    private RectTransform rect;
    private UI ui;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statName;
    [SerializeField] private TextMeshProUGUI statValue;

    private void OnValidate()
    {
        gameObject.name = "数据-" + statType.GetChineseName();
        statName.text = statType.GetChineseName();
    }
    private void Awake()
    {
        ui = GetComponentInChildren<UI>();
        rect = GetComponent<RectTransform>();
        playerStats = FindFirstObjectByType<Entity_Stats>();
    }
    public void UpdateStatValue()
    {
        Stat statNeedUpdate = playerStats.GetStatByType(statType);
        if (statNeedUpdate == null && statType != StatType.ElementDamage)//元素伤害没有专属数据不能通过种类查找，所以为空，但应该参加下面的查找得到具体值，即只有出现其他异常种类才返回null
            return;
        float value = 0;
        switch (statType)//用于得到各项具体数值
        {
            case StatType.Strength:
                value = playerStats.major.strength.GetValue(); break;
            case StatType.Agility:
                value = playerStats.major.agility.GetValue(); break;
            case StatType.Intelligence:
                value = playerStats.major.intelligence.GetValue(); break;
            case StatType.Vitality:
                value = playerStats.major.vitality.GetValue(); break;

            case StatType.MaxHP:
                value = playerStats.GetMaxHp(); break;
            case StatType.HpRegen:
                value = playerStats.resources.HpRegen.GetValue(); break;
            case StatType.Evasion:
                value = playerStats.GetEvasion(); break;
            case StatType.Armor:
                value = playerStats.GetBaseArmor(); break;

            case StatType.Damage:
                value = playerStats.GetBaseDamage(); break;
            case StatType.CritChance:
                value = playerStats.GetCritChance(); break;
            case StatType.CritPower:
                value = playerStats.GetCritPower(); break;
            case StatType.ArmorReduction:
                value = playerStats.GetArmorReduction() * 100; break;
            case StatType.AttackSpeed:
                value = playerStats.attack.attackSpeed.GetValue() * 100; break;

            case StatType.FireDamage:
                value = playerStats.attack.fireDamage.GetValue(); break;
            case StatType.IceDamage:
                value = playerStats.attack.iceDamage.GetValue(); break;
            case StatType.LightningDamage:
                value = playerStats.attack.lightningDamage.GetValue(); break;
            case StatType.ElementDamage:
                value = playerStats.GetElementalDamage(out ElementType element, 1); break;

            case StatType.FireResistance:
                value = playerStats.GetElementalResistance(ElementType.Fire) * 100; break;
            case StatType.IceResistance:
                value = playerStats.GetElementalResistance(ElementType.Ice) * 100; break;
            case StatType.LightningResistance:
                value = playerStats.GetElementalResistance(ElementType.Lightning) * 100; break;
        }
        statValue.text = IsPercentStat(statType) ? value + "%" : value.ToString();
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
