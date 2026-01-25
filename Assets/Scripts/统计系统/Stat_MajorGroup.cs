using System;
using UnityEngine;


[Serializable]
public class Stat_MajorGroup//设定额外数值
{
    [cn("力量")] public Stat strength;//力量
    [cn("敏捷")] public Stat agility;//敏捷
    [cn("智力")] public Stat intelligence;//智力
    [cn("活力")] public Stat vitality;//每点活力值提升5点最大生命值
}
