using UnityEngine;

public class Archer_BattleState : Enemy_BattleState
{
    private bool canFlip;
    private bool shouldStop;
    public Archer_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        shouldStop = false;
        canFlip = true;//默认为true可以保证受击进入战斗时正常翻转
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimator();

        if (enemy.isDetectGround == false || enemy.isDetectWall)
            shouldStop = true;

        if (enemy.DetectPlayer() == true)//检测到玩家刷新战斗时间
        {
            UpdateTarget();
            lastTimeWasInBattle = Time.time;
        }

        if (BattleTimeOver())//战斗时间结束返回空闲状态
            stateMachine.ChangeState(enemy.idleState);

        if (CanAttack())
        {
            enemy.SetVelocity(0, rb.linearVelocity.y);

            if (enemy.DetectPlayer() == false & canFlip)
            {
                enemy.HandleFlip(DirectionToPlayer());//直接使用Flip()的话，当玩家跑开攻击范围后，会触发翻转，而HandleFlip()不会
                canFlip = false;
            }

            if (enemy.DetectPlayer() && InAttackRange())
            {
                canFlip = true;
                lastTimeAttack = Time.time;
                stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            bool DistanceSafe = GetDistanceToPlayer() > (enemy.attackDistance * 0.8f);
            bool shouldWalkAway = (shouldStop == false && DistanceSafe == false);

            if (shouldWalkAway)
                enemy.SetVelocity(enemy.GetBattleMoveSpeed() * -1 * DirectionToPlayer(), rb.linearVelocity.y);
            else
            {
                enemy.SetVelocity(0, rb.linearVelocity.y);
                if (enemy.DetectPlayer() == false)
                    enemy.HandleFlip(DirectionToPlayer());//直接使用Flip()的话，当玩家跑开攻击范围后，会触发翻转，而HandleFlip()不会
            }
        }
    }
}
