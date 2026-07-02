using UnityEngine;

public class Archer_BattleState : Enemy_BattleState
{
    private bool canFlip;
    private bool shouldStop;
    private Enemy_Archer archer;
    public Archer_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        archer = enemy as Enemy_Archer;
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

        //判断是否是区域触发器模式
        bool useTriggerMode = archer != null && archer.enableTracking && archer.useTriggerZoneAttack && archer.triggerZone != null;
        bool playerInTriggerZone = false;
        if (useTriggerMode && Player.instance != null)
        {
            Vector2 playerPos = Player.instance.transform.position;
            playerInTriggerZone = archer.triggerZone.OverlapPoint(playerPos);
        }

        //战斗时间刷新
        bool playerDetectedByRay = enemy.DetectPlayer();
        bool playerInRange = playerDetectedByRay || (useTriggerMode && playerInTriggerZone); // 关键：合并检测

        if (playerInRange)
        {
            UpdateTarget();
            lastTimeWasInBattle = Time.time;
        }

        if (BattleTimeOver())
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // ---------- 攻击逻辑 ----------
        if (CanAttack())
        {
            enemy.SetVelocity(0, rb.linearVelocity.y);

            //攻击条件
            bool shouldAttack = useTriggerMode ? playerInTriggerZone : (playerDetectedByRay && InAttackRange());

            //翻转处理
            if (useTriggerMode && playerInTriggerZone)
            {
                enemy.HandleFlip(DirectionToPlayer());
                canFlip = true;
            }
            else if (!playerDetectedByRay && canFlip && !useTriggerMode)
            {
                enemy.HandleFlip(DirectionToPlayer());
                canFlip = false;
            }

            if (shouldAttack)
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
    protected override bool BattleTimeOver()
    {
        //如果启用了永久战斗，永不超时
        if (archer != null && archer.keepBattleForever)
            return false;
        //否则沿用基类的计时逻辑
        return base.BattleTimeOver();
    }
}
