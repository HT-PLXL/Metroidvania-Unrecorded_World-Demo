using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (player.canJumpTwo && input.Player.Jump.WasPressedThisFrame())
        {
            player.SetVelocity(0, player.jumpForce);
            player.canJumpTwo = false;
        }

        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
            //父类是空中状态，父类的update检测并执行Changestate（）进入空中斩击状态
            //而changestate（）会在同一帧执行exit（）和enter（）
            //所以在这一帧会执行跳跃斩击的enter（）同时在此处对y速度判定是否进入下落状态
            //而跳跃斩击的enter（）中设置了负值的y速度加速下降
            //从而导致此处检测到了负值y意外进入下落状态
            //起跳时攻击，状态变化为：跳跃-跳跃斩击-下落
            //因此需要补充判定——不处于跳跃斩击状态
            stateMachine.ChangeState(player.fallState);
    }
}
