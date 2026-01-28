using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum, Inherited = true)]
public class cnAttribute : PropertyAttribute
{
    public readonly string ChineseName;
    public cnAttribute(string chineseName) => ChineseName = chineseName;
}