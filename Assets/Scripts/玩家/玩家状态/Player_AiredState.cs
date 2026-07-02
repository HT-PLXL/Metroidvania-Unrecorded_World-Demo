using UnityEngine;

public class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        if (player.moveInput.x != 0)
            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.airMoveMultiplier), rb.linearVelocity.y);

        if (input.Player.普通攻击.WasPressedThisFrame())
        {
            if (player.moveInput.y < 0)
                stateMachine.ChangeState(player.jumpAttackState);//下落攻击
            else if (player.airComboExhausted == false)
                stateMachine.ChangeState(player.airComboState);//空中连击
        }
    }
}
