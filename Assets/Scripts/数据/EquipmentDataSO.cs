using System;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/装备", fileName = "装备-")]

public class EquipmentDataSO : ItemDataSO
{
    [Header("武器元素（仅武器有效）")]
    public ElementType weaponElement;
    [Header("物品修改")]
    public ItemModifier[] modifier;//需要修改的属性
    [Header("免疫效果")]
    public bool immunePhysical;
    public bool immuneFire;
    public bool immuneIce;
    public bool immuneLightning;
}

[Serializable]
public class ItemModifier//新类
{ 
    public StatType statType;
    public float value;
}
