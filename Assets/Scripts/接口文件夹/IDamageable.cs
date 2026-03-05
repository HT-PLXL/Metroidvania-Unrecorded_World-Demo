using UnityEngine;
//接口“可受伤”,挂载在实体血量上，或者箱子这类可交互实体上，所有挂载了此接口的对象都要完善对应的TakeDamage，可以是任何想在受伤时做的内容，包括扣血功能
public interface IDamageable
{
    public bool TakeDamage(float damagef,float elementalDamage,ElementType element, Transform damageSource);//damageSource伤害来源
}
