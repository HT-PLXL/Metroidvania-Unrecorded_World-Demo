using System;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    protected Transform player;
    protected Transform lastTarget;
    protected float lastTimeWasInBattle;
    protected float lastTimeAttack = -999;
    public bool canNotJumpBack;
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Update()
    {
        base.Update();

        if (enemy.DetectPlayer() == true)//检测到玩家刷新战斗时间
        {
            UpdateTarget();
            lastTimeWasInBattle = Time.time;
        }

        if (BattleTimeOver())//战斗时间结束返回空闲状态
            stateMachine.ChangeState(enemy.idleState);

        if (InAttackRange() && enemy.DetectPlayer() && CanAttack())
        {
            lastTimeAttack = Time.time;
            stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            float xVelocity = enemy.ShouldChasePlayer ? enemy.GetBattleMoveSpeed()  : 0.0001f;//使用一个极小数，确保几乎不动的同时也能正常翻转
            enemy.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocity.y);
        }
    }
    public override void Enter()
    {
        base.Enter();

        lastTimeWasInBattle = Time.time;
        //如果被玩家打却没有检测到玩家(比如远程攻击)，就需要直接按这里的计时等待结束战斗状态
        //虽然这还是会让小怪“快速罚站”哈哈
        //不加会在游戏时间大于持续时间后每次触发都让小怪直接进入空闲状态然后等待进入移动状态
        //如果是正常发现玩家，Update里每帧更新计时维持战斗状态


        player ??= enemy.GetPlayerTransform();
        //??.是空合并赋值运算符，如果变量为空则赋值，不为空则不执行

        if (ShouldBackJump()&& canNotJumpBack)
        {
            rb.linearVelocity = new Vector2(enemy.backJumpSpeed.x * enemy.activeSlowDownmultiplier * -DirectionToPlayer(), enemy.backJumpSpeed.y);
            enemy.HandleFlip(DirectionToPlayer());
        }
    }
    protected  bool CanAttack() => Time.time > lastTimeAttack + enemy.attackCooldown;//冷却结束才可攻击
    protected float GetDistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;//极大值
        return MathF.Abs(player.position.x - enemy.transform.position.x);
        //MathF是System命名空间下的一个静态类，专门用于处理单精度浮点数（float 类型）
        //MathF无法实例化，所有成员（方法、常量）均为静态，需通过MathF.XXX直接调用。
        //Abs：absolute，这里是绝对值运算
    }
    protected bool BattleTimeOver() => Time.time > lastTimeWasInBattle + enemy.battleTimeDuration;
    // =>会根据 判断成立与否 返回值
    protected bool InAttackRange()
    {
        return GetDistanceToPlayer() < enemy.attackDistance;
        //条件判断，小于则返回true，否则返回false
        //另一种写法：private bool InAttackRange() => GetDistanceToPlayer() < enemy.attackDistance;
    }
    protected bool ShouldBackJump() => GetDistanceToPlayer() < enemy.minBackJumpDistance;
    protected int DirectionToPlayer()
    {
        if (player == null)
            return 0;
        return player.position.x > enemy.transform.position.x ? 1 : -1;
        //Animator界面中参数xVelocity需要临界值0，1与-1
    }
    protected void UpdateTarget()
    {
        if (enemy.DetectPlayer() == false)
            return;
        Transform newTarget = enemy.DetectPlayer().transform;
        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

}
