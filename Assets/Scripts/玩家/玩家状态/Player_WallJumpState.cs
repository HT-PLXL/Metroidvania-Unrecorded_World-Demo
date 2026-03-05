using UnityEngine;

public class Player_WallJumpState : PlayerState
{
    public Player_WallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    //重要，此处依旧有bug，偶现于贴右墙时左+空格，有概率原地跳一下，待解决
    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(player.wallJumpForce.x * -player.faceDirection, player.wallJumpForce.y);
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        //if(player.isDetectWall)
        //    stateMachine.ChangeState(player.wallSlideState);

    }
}
