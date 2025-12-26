using System;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;//让每个外界声明的Stat类实例都拥有在unity中可视可改的值
    public float GetValue()
    {
        return baseValue;
    }
}
