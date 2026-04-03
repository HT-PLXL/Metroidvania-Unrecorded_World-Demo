using System;
using UnityEngine;
[Serializable]
public class Stat_AttackGroup
{
    [cn("攻击速度")] public Stat attackSpeed;

    [cn("物理伤害")] public Stat damage;
    [cn("暴击率")] public Stat critChance;
    [cn("暴击伤害倍率(1+基础值/100)")] public Stat critPower;
    [cn("护甲削减")] public Stat armorReduction;//护甲削减
    //元素伤害
    [cn("火焰伤害")] public Stat fireDamage;
    [cn("冰霜伤害")] public Stat iceDamage;
    [cn("雷电伤害")] public Stat lightningDamage;//闪电伤害
}
