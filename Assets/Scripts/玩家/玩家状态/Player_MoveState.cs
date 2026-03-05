using UnityEngine;

public class Player_MoveState : Player_GroundState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }
    public override void Update()
    {
        base.Update();
        if (player.moveInput.x == 0 || player.isDetectWall)
            stateMachine.ChangeState(player.idleState);//x轴速度为0转入空闲
        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);//实现移动
    }
}
