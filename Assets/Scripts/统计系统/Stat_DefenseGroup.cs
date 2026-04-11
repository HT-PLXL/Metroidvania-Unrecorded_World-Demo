using System;
using UnityEngine;
[Serializable]
public class Stat_DefenseGroup
{
    [cn("物理护甲")] public Stat armor;//护甲
    [cn("闪避率")] public Stat evasion;//闪避率

    //元素抵抗
    [cn("火焰抗性")] public Stat fireResistance;
    [cn("冰霜抗性")] public Stat iceResistance;
    [cn("雷电抗性")] public Stat lightningResistance;
}
