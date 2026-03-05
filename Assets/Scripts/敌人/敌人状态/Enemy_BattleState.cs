using System;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private Transform lastTarget;
    private float lastTimeWasInBattle;
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Update()
    {
        base.Update();

        if (enemy.detectPlayer() == true)
        {
            UpdateTarget();
            lastTimeWasInBattle = Time.time;
        }
        if (战斗时间结束())
            stateMachine.ChangeState(enemy.idleState);

        if (在攻击距离内() && enemy.detectPlayer())
            stateMachine.ChangeState(enemy.attackState);
        else
            enemy.SetVelocity(enemy.GetBattleMoveSpeed() * 朝向玩家的方向(), rb.linearVelocity.y);
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

        if (应该后撤())
        {
            rb.linearVelocity = new Vector2(enemy.backJumpSpeed.x * enemy.activeSlowDownmultiplier * -朝向玩家的方向(), enemy.backJumpSpeed.y);
            enemy.HandleFlip(朝向玩家的方向());
        }
    }

    private float 与玩家的距离()
    {
        if (player == null)
            return float.MaxValue;//极大值
        return MathF.Abs(player.position.x - enemy.transform.position.x);
        //MathF是System命名空间下的一个静态类，专门用于处理单精度浮点数（float 类型）
        //MathF无法实例化，所有成员（方法、常量）均为静态，需通过MathF.XXX直接调用。
        //Abs：absolute，这里是绝对值运算
    }
    private bool 战斗时间结束() => Time.time > lastTimeWasInBattle + enemy.battleTimeDuration;
    // =>会根据 判断成立与否 返回值
    private bool 在攻击距离内()
    {
        return 与玩家的距离() < enemy.attackDistance;
        //条件判断，小于则返回true，否则返回false
        //另一种写法：private bool 在攻击距离内() => 与玩家的距离() < enemy.attackDistance;
    }
    private bool 应该后撤() => 与玩家的距离() < enemy.minBackJumpDistance;
    private int 朝向玩家的方向()
    {
        if (player == null)
            return 0;
        return player.position.x > enemy.transform.position.x ? 1 : -1;
        //Animator界面中参数xVelocity需要临界值0，1与-1
    }
    private void UpdateTarget()
    {
        if (enemy.detectPlayer() == false)
            return;
        Transform newTarget = enemy.detectPlayer().transform;
        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }
}
