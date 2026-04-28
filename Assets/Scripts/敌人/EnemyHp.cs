using UnityEngine;

public class EnemyHp : EntityHp
{
    public bool canEnterBattleState { get; set; } = true;//非常重要，限制进入战斗状态，防止顶状态
    private Enemy enemy => GetComponent<Enemy>();
    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageSource, bool isCrit = false)
    {
        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageSource,isCrit);//在父类获取是否真的被击中，用于判断

        if (wasHit == false)
            return false;//返回值用于实体战斗中判定是否启用攻击特效

        if (damageSource.GetComponent<Player>() != null && canEnterBattleState)//确认组件中包含Player脚本，有则说明它是玩家
            enemy.TryEnterCombatState(damageSource);//注意这里进战斗状态会顶掉其他状态，动画来不及播放，需要关闭状态机或者canEnterBattleState =false
        return true;//返回值用于实体战斗中判定是否启用攻击特效
    }
    protected override void Die()
    {
        base.Die();

        // 如果该敌人不可重生，立即记录死亡
        if (SaveManager.instance != null && enemy.isRebirthOnLoad == false)
            SaveManager.instance.MarkEnemyDead(enemy.GetEnemyId());
    }
}
