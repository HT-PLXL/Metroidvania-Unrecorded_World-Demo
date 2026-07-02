using System;
using UnityEngine;

[Serializable]
public class DamageScaleFactor//缩放因子类，用于给攻击/技能设定伤害倍率
{
    [Header("伤害倍率")]
    [cn("物理伤害倍率")] public float physical = 1;
    [cn("元素伤害倍率")] public float elemental = 1;
    [Header("霜冻")]
    [cn("减速时间")] public float chillDuration = 3;
    [cn("减速效果倍率")] public float chillSlowMulitiplier = 0.35f;
    [Header("灼烧")]
    [cn("灼烧时间")]public float burnDuration = 3;
    [cn("灼烧伤害倍率")]public float burnDamageScale = 3;
    [Header("雷击")]
    [cn("雷击窗口时间")]public float lightningDuration = 3;
    [cn("雷击伤害倍率")]public float lightningDamageScale = 1;
    [cn("雷击充能值")]public float lightningCharge = 0.4f;
}
