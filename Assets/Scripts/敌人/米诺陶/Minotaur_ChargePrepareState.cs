using UnityEngine;

public class Minotaur_ChargePrepareState : EnemyState
{
    private Minotaur minotaur;

    public Minotaur_ChargePrepareState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
        minotaur = enemy as Minotaur;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetVelocity(0, rb.linearVelocity.y);
        // 朝向玩家
        if (minotaur.player != null)
        {
            int dir = minotaur.DirectionToPlayer();
            if (dir != 0) minotaur.HandleFlip(dir);
        }
    }

    public override void Update()
    {
        base.Update();
        // 等待动画事件 triggerOver 触发后切换到冲撞状态
        if (triggerOver)
        {
            stateMachine.ChangeState(minotaur.chargeState);
        }
    }
}