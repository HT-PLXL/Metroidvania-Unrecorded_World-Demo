using UnityEngine;
//怪物——骷髅脚本
public class Skeleton : Enemy , ICounter
{
    public bool CanBeCountered { get => canBeStunned;}

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        //地面状态索敌会转入battleState状态，所以新角色需要给battle赋值，如battleState = ArcherBattleState;

        haveDeadAnim = true;
    }
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }
    public void HandleCounter()//实现接口
    {
        stateMachine.ChangeState(stunnedState);
    }
    public override void AfterDie()
    {
        DelayDestroyEnemy(2);
    }
}
