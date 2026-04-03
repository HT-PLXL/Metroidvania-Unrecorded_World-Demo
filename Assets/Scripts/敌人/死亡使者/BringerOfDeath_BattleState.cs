using UnityEngine;

public class BringerOfDeath_BattleState : Enemy_BattleState
{
    private Enemy_BringerOfDeath bringerOfDeath;
    public BringerOfDeath_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        bringerOfDeath = enemy as Enemy_BringerOfDeath;
    }
    public override void Enter()
    {
        base.Enter();
        stateTimer = bringerOfDeath.maxBattleIdleTime;
    }
    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimator();
        if (stateTimer < 0)
            stateMachine.ChangeState(bringerOfDeath.bringerOfDeath_TeleportState);
        if (enemy.DetectPlayer() == true)
            UpdateTarget();

        if (InAttackRange() && enemy.DetectPlayer() && CanAttack())
        {
            lastTimeAttack = Time.time;
            stateMachine.ChangeState(bringerOfDeath.bringerOfDeath_AttackState);
        }
        else
        {
            float xVelocity = enemy.isDetectGround ? enemy.GetBattleMoveSpeed() : 0.0001f;//使用一个极小数，确保几乎不动的同时也能正常翻转
            enemy.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocity.y);//速度极小时，事实上混合树会使其播放空闲动画,而且位置也不再移动了
        }
    }
}
