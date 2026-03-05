using UnityEngine;

public class Enemy_MoveState : Enemy_GroundState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        if (enemy.isDetectGround == false || enemy.isDetectWall)
            enemy.Flip();
    }
    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.faceDirection, rb.linearVelocity.y);
        if(enemy.isDetectGround == false || enemy.isDetectWall)
            stateMachine.ChangeState(enemy.idleState);
    }

}
