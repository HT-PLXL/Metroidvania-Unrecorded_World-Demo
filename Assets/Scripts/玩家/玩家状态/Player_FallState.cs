public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.HadUnlockDoubleJump() && player.canJumpTwo && input.Player.Jump.WasPressedThisFrame())
        {
            player.SetVelocity(0, player.jumpForce);//二段
            player.canJumpTwo = false;
        }

        if (player.isDetectGround)
            stateMachine.ChangeState(player.idleState);
        if (player.isDetectWall)
        {
            player.canJumpTwo = true;
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
