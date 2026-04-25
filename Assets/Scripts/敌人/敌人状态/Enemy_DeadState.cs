using UnityEngine;
//敌人通用的死亡状态，可以应对没有死亡动画的角色素材
//无死亡动画素材的角色只需 deadState = new Enemy_DeadState(this, stateMachine, "idle");即进入死亡状态但动画切为空闲
//有死亡动画只需将haveDeadAnim设为True然后deadState = new Enemy_DeadState(this, stateMachine, "dead");
public class Enemy_DeadState : EnemyState
{
    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.haveDeadAnim)//有专属死亡动画
        {
            stateMachine.DisableStateMachine();//关闭状态机不允许再切换状态
            rb.simulated = false;//停止物理模拟
            enemy.col.enabled = false;//关闭碰撞体，动画期间不允许再被攻击
            enemy.enemyHp?.EnableHpBar(false);//关闭血条
            enemy.AfterDie();//额外处理内容
        }
        else
        {
            anim.enabled = false;
            enemy.col.enabled = false;
            rb.gravityScale = 12;//增大重力
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);
            stateMachine.DisableStateMachine();//关闭状态机
            enemy.DelayDestroyEnemy();//延迟销毁
        }
    }
}
