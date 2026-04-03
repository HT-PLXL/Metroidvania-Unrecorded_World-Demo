using UnityEngine;

public class Enemy_StunnedState : EnemyState
{
    private Enemy_VFX VFX;
    public Enemy_StunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        VFX = enemy.GetComponent<Enemy_VFX>();
    }
    public override void Enter()
    {
        base.Enter();

        VFX.PlayAttackWarning(false);
        enemy.EnableCounterWindow(false);
        //攻击动画在弹反时被取消了，第二个取消反击窗口的节点就不会被触发，因此需要在这里取消
        stateTimer =enemy.stunnedDuration;
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.faceDirection, enemy.stunnedVelocity.y);
    }
    public override void Update()
    {
        base.Update();
        if (stateTimer < 0 && enemy.stunRecovery == false)//僵直时间结束且未启用僵直恢复动画
            stateMachine.ChangeState(enemy.idleState);
    }
}
