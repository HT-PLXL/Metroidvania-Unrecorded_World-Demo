using UnityEngine;

public enum StatType
{
    [cn("最大血量")]MaxHP,
    [cn("生命回复")] HpRegen,
    [cn("力量")] Strength,
    [cn("敏捷")] Agility,
    [cn("智力")] Intelligence,
    [cn("活力")] Vitality,
    [cn("攻击速度")] AttackSpeed,
    [cn("物理伤害")] Damage,
    [cn("暴击率")] CritChance,
    [cn("暴击伤害")] CritPower,
    [cn("护甲削减")] ArmorReduction,
    [cn("火焰伤害")] FireDamage,
    [cn("冰霜伤害")] IceDamage,
    [cn("雷电伤害")] LightningDamage,
    [cn("物理护甲")] Armor,
    [cn("闪避率")] Evasion,
    [cn("冰霜抗性")] IceResistance,
    [cn("火焰抗性")] FireResistance,
    [cn("雷电抗性")] LightningResistance,
    [cn("元素伤害")] ElementDamage
}
