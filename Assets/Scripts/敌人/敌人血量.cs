using UnityEngine;

public class 敌人血量 : EntityHp
{
    private Enemy enemy => GetComponent<Enemy>();//会多次调用，所以写在Awake中会更好
    public override bool 造成伤害(float damage, float elementalDamage, ElementType element, Transform 伤害来源)
    {
        bool wasHit = base.造成伤害(damage,elementalDamage,element, 伤害来源);//在父类获取是否真的被击中，用于判断

        if (wasHit == false)
            return false;//返回值用于实体战斗中判定是否启用攻击特效
        //if (伤害来源.CompareTag("Player"))//比对玩家面板中的Tag标签，方法二如下
        if (伤害来源.GetComponent<Player>() != null )//确认组件中包含Player脚本，有则说明它是玩家
            enemy.受击进入战斗状态(伤害来源);
            return true;//返回值用于实体战斗中判定是否启用攻击特效
    }
}
