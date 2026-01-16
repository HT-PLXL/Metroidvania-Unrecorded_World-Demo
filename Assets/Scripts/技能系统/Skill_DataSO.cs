using System;
using UnityEngine;
//此脚本实现技能数据可自定义对象
[CreateAssetMenu(menuName = "游戏设置/技能数据", fileName = "技能数据")]

public class Skill_DataSO : ScriptableObject
{
    [Header("技能信息设定")]
    [cn("技能名字")] public string skillName;
    [cn("解锁需要技能点")] public int cost;
    [TextArea]public string description;
    [cn("技能图标")] public Sprite icon;
    [cn("默认解锁？")] public bool unlockedByDefault;
    [cn("技能种类")] public SkillType skillType;
    [cn("升级详情")] public UpgradeData upgradeData;
}
[Serializable]
public class UpgradeData
{
    [cn("升级种类")] public SkillUpgradeType upgradeType;
    [cn("冷却时间")] public float cooldown;
    [cn("伤害比例")] public DamageScaleFactor damageScale;
}