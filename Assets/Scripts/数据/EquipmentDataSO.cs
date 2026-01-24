using System;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/装备", fileName = "装备-")]

public class EquipmentDataSO : ItemDataSO
{
    [Header("物品修改")]
    public ItemModifier[] modifier;//需要修改的属性
}

[Serializable]
public class ItemModifier//新类
{ 
    public StatType statType;
    public float value;
}
