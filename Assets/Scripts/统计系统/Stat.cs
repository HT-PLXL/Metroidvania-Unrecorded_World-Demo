using System;
using UnityEngine;

[Serializable]
public class Stat//这个类用于在unity面板设置基础值，其他脚本使用值时使用的是在Entity_stats包装一层（属性点加成）后的值
{
    [SerializeField] private float baseValue;//让每个外界声明的Stat类实例都拥有在unity中可视可改的值
    public float GetValue()
    {
        return baseValue;
    }
}
