using UnityEngine;

[CreateAssetMenu(menuName = "生物数据设置/默认生物数据设置",fileName = "数据设置-")]
public class Stat_SetupSO : ScriptableObject
{
    [Header("血量相关")]
    public float maxHp = 100;
    public float HpRegen;
    [Header("物理伤害")]
    public float attackSpeed = 1;
    public float damage = 10;
    public float critChange;
    public float critPower;
    public float armorReduction;
    [Header("元素伤害")]
    public float fireDamage;
    public float iceDamage;
    public float lightningDamage;
    [Header("物理防御")]
    public float armor;
    public float evasion;
    [Header("元素抵抗")]
    public float fireResistance;
    public float iceResistance;
    public float LightningResistance;
    [Header("核心加成数据")]
    public float strength;
    public float agility;
    public float intelligence;
    public float vitality;

}
