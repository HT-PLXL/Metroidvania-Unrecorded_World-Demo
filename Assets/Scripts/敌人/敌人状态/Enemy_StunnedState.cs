using UnityEngine;

public class Enemy_StunnedState : EnemyState
{
    private 敌人视觉效果 VFX;
    public Enemy_StunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        VFX = enemy.GetComponent<敌人视觉效果>();
    }
    public override void Enter()
    {
        base.Enter();

        VFX.可弹出攻击警告(false);
        enemy.EnableCounterWindow(false);
        //攻击动画在弹反时被取消了，第二个取消反击窗口的节点就不会被触发，因此需要在这里取消
        stateTimer =enemy.stunnedDuration;
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.faceDirection, enemy.stunnedVelocity.y);
    }
    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}
