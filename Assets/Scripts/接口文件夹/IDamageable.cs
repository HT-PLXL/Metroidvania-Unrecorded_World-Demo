using UnityEngine;

public interface IDamageable//接口“可受伤”
    //接口允许创建无实现（无花括号部分）的方法
    //当类附加了接口的时候，就必须实现接口的功能，相当于一种约束
{
    public bool 造成伤害(float damagef,float elementalDamage,ElementType element, Transform damageDealer);
    //“实体血量”和“箱子”中附加了接口，它在“实体攻击目标检测”获得被攻击者的组件后
    //执行“造成伤害”这个方法，所有被攻击检测到的对象都将执行这个方法
    //但箱子有自己的类，且继承的基类独立于正常实体，执行自己的“造成伤害”方法
    //也就实现了不同的对象执行同一个方法达成不同效果
}
