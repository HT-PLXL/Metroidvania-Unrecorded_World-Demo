using UnityEngine;

public class Enemy_MoveState : Enemy_GroundState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        if (enemy.检测到地面 == false || enemy.检测到墙壁)
            enemy.Flip();
    }
    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(enemy.moveSpeed * enemy.面对方向, rb.linearVelocity.y);
        if(enemy.检测到地面 == false || enemy.检测到墙壁)
            stateMachine.ChangeState(enemy.idleState);
    }

}
