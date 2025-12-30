using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat//这个类用于在unity面板设置基础值，其他脚本使用值时使用的是在Entity_stats包装一层（属性点加成）后的值
{
    [SerializeField] private float baseValue;//让每个外界声明的Stat类实例都拥有在unity中可视可改的值
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();
    private float finalvalue;
    private bool needCalculate = true;
    public float GetValue()
    {
        //return GetfinalValue();性能不友好，优化如下
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

}

[Serializable]
public class StatModifier
{
    public float value;
    public string source;//来源

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}

