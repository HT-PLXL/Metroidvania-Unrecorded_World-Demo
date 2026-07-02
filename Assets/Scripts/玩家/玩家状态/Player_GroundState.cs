using UnityEngine;

public class Player_GroundState : PlayerState
{
    public Player_GroundState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();

        //落地时重置空中连招限制
        player.airComboExhausted = false;
    }
    public override void Update()
    {
        base.Update();
        if (rb.linearVelocity.y < 0 && player.isDetectGround == false)
            stateMachine.ChangeState(player.fallState);

        //输入PlayerInputSet界面填入的名称
        if (input.Player.Jump.WasPressedThisFrame())//WasPressedThisFrame是新输入系统PlayerInputSet脚本定义的
        {
            player.canJumpTwo = true;
            stateMachine.ChangeState(player.jumpState);
        }

        if (input.Player.普通攻击.WasPressedThisFrame())
            stateMachine.ChangeState(player.attackState);
        if (input.Player.弹反.WasPressedThisFrame())
            stateMachine.ChangeState(player.counterAttactState);
        if (input.Player.掷剑.WasPressedThisFrame()&& skillManager.throwSword.CanUseSkill())
            stateMachine.ChangeState(player.throwSwordState);
    }
}
