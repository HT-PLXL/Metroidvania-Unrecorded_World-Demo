using UnityEngine;

public class Minotaur_BattleState : Enemy_BattleState
{
    private Minotaur minotaur;

    public Minotaur_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
        minotaur = enemy as Minotaur;
    }

    public override void Enter()
    {
        base.Enter();
        player ??= enemy.GetPlayerTransform();
        minotaur.enemyHp.canEnterBattleState = false;//进入战斗状态就不再允许受击进入

        if (minotaur.boosHpBar.gameObject.activeSelf == false)
        {
            minotaur.boosHpBar.SetTarget(minotaur, "米诺陶");
        }
    }

    public override void Update()
    {
        UpdateAnimator();

        if (enemy.DetectPlayer())
        {
            UpdateTarget();
            lastTimeWasInBattle = Time.time;
        }

        if (BattleTimeOver())
        {
            minotaur.enemyHp.canEnterBattleState = true;//仅当战斗超时自动退出时才允许启用受击进入
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        DecideAndPerformAttack();

        if (stateMachine.currentState != this)
            return;

        if (minotaur.player == null)
        {
            Transform newPlayer = enemy.GetPlayerTransform();
            if (newPlayer != null)
                minotaur.player = newPlayer;
            if (minotaur.player == null)
                return;
        }

        float distanceToPlayer = Vector2.Distance(minotaur.transform.position, minotaur.player.position);
        if (enemy.ShouldChasePlayer && distanceToPlayer > 1.5f)
        {
            float xVelocity = enemy.GetBattleMoveSpeed() * DirectionToPlayer();
            // 如果前方有墙且正在向墙移动，停止移动
            if (enemy.isDetectWall && Mathf.Sign(xVelocity) == enemy.faceDirection)
            {
                return;
            }
            enemy.SetVelocity(xVelocity, rb.linearVelocity.y);
        }
    }

    private void DecideAndPerformAttack()
    {
        float dist = minotaur.DistanceToPlayer();

        // 冲撞准备（冷却满足且距离合适）
        if (dist >= minotaur.chargeMinDistance && dist <= minotaur.chargeMaxDistance &&
            Time.time > minotaur.lastChargeTime + minotaur.chargeCooldown)
        {
            minotaur.lastChargeTime = Time.time;
            stateMachine.ChangeState(minotaur.chargePrepareState);
            return;
        }

        // 跺脚（近身范围）
        if (dist <= minotaur.attackDistance && Time.time > minotaur.lastStompTime + minotaur.stompCooldown)
        {
            minotaur.lastStompTime = Time.time;
            stateMachine.ChangeState(minotaur.stompState);
            return;
        }

        // 旋转横扫
        if (dist <= minotaur.spinRadius + 0.5f && Time.time > minotaur.lastSpinTime + minotaur.spinCooldown)
        {
            minotaur.lastSpinTime = Time.time;
            stateMachine.ChangeState(minotaur.spinState);
            return;
        }

        // 劈砍
        if (dist <= minotaur.attackDistance && Time.time > minotaur.lastSlashTime + minotaur.slashCooldown)
        {
            minotaur.lastSlashTime = Time.time;
            stateMachine.ChangeState(minotaur.slashState);
            return;
        }

        // 普通攻击
        if (dist <= minotaur.attackDistance && Time.time > minotaur.lastBasicAttackTime + minotaur.basicCooldown)
        {
            minotaur.lastBasicAttackTime = Time.time;
            stateMachine.ChangeState(minotaur.basicAttackState);
        }
    }
}