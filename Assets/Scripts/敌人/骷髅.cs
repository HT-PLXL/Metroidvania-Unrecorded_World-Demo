using UnityEngine;

public class 骷髅 : Enemy , 弹反
{
    public bool CanBeCountered { get => canBeStunned;}
    //读取CanBeCounter时，返回类中canBeStunned的值

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }
    protected override void Start()
    {
        base.Start();

        stateMachine.Initalize(idleState);
    }
    public void HandleCounter()//实现接口
    {
        stateMachine.ChangeState(stunnedState);
    }
}
