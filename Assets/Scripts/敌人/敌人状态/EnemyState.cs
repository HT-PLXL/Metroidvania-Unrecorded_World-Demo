using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;
    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;

        rb = enemy.rb;
        anim = enemy.anim;
        entity_stats = enemy.stats;
    }

    public override void UpdateAnimator()
    {
        base.UpdateAnimator();

        float moveAndAnimSpeedRatioOnCombat = enemy.battleMoveSpeed / enemy.moveSpeed;
        anim.SetFloat("战斗时移动与动画速率比", moveAndAnimSpeedRatioOnCombat);
        anim.SetFloat("移动与动画速率比", enemy.moveAndAnimSpeedRatio);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);//混合树参数在battle状态切换idle/move动画
    }
}
