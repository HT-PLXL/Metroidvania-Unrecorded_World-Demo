using UnityEngine;

public class Player_WallSlideState : PlayerState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        HandleFlip();//涉及翻转，所以代码必须在切换墙壁跳跃状态之上

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.wallJumpState);

        if (player.isDetectWall == false)
            stateMachine.ChangeState(player.fallState);

        if (player.isDetectGround)
            stateMachine.ChangeState(player.idleState);



    }
    private void HandleFlip()
    {
        if (player.moveInput.y < 0)
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
            //player中有根据x轴及“isFacingRight”进行角色翻转的功能，因此这里无需担心
        else
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.WallSlideMultiplier);

    }
}
