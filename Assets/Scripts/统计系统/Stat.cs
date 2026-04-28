using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat//这个类用于在unity面板设置基础值，其他脚本使用值时使用的是在Entity_stats包装一层（属性点加成）后的值
{
    [SerializeField][cn("基础值")] private float baseValue;//让每个外界声明的Stat类实例都拥有在unity中可视可改的值
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();
    private float finalvalue;
    private bool needCalculate = true;
    public float GetValue()
    {
        if (needCalculate)
        {
            finalvalue = GetfinalValue();
            needCalculate = false;
        }
        return finalvalue;
    }
    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        needCalculate = true;
    }
    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);//清除所有（任意名=>任意名.source == 括号传入的source），括号内为真执行清除
        needCalculate = true;
    }
    public float GetfinalValue()
    {
        float finalValue = baseValue;
        foreach (var modifier in modifiers)
        {
            finalValue = finalValue + modifier.value;
        }
        return finalValue;
    }
    public void SetBaseValue(float value)
    {
        baseValue = value;
        needCalculate = true;
    }
}

[Serializable]
public class StatModifier
{
    [cn("附加值")] public float value;
    [cn("来源")] public string source;//来源

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}

