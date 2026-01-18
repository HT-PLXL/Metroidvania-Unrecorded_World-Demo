using UnityEngine;

public class Player_WallSlideState : PlayerState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        滑墙处理();//涉及翻转，所以代码必须在切换墙壁跳跃状态之上

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.wallJumpState);

        if (player.检测到墙壁 == false)
            stateMachine.ChangeState(player.fallState);

        if (player.检测到地面)
            stateMachine.ChangeState(player.idleState);



    }
    private void 滑墙处理()
    {
        if (player.moveInput.y < 0)
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
            //player中有根据x轴及“面向右方”进行角色翻转的功能，因此这里无需担心
        else
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.滑墙下降倍率);

    }
}
