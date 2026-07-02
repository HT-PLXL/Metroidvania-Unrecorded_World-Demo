using UnityEngine;
//此类用于汇总数据，方便调用
[SerializeField]
public class AttackData
{
    public float physicalDamage;
    public float elementalDamage;
    public bool isCrit;
    public ElementType element;
    public ElementEffectData effectData;

    public AttackData(Entity_Stats entityStats,DamageScaleFactor damageScale)
    {
        physicalDamage = entityStats.GetPhysicalDamage(out isCrit, damageScale.physical);
        elementalDamage = entityStats.GetElementalDamage(out element,damageScale.elemental);
        effectData = new ElementEffectData(entityStats,damageScale);//传入自身的各种攻击数据，基础攻击比例
    }
}
