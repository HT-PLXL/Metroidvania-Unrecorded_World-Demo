using UnityEngine;

public class Player_GroundState : PlayerState
{
    public Player_GroundState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();
        if (rb.linearVelocity.y < 0 && player.检测到地面 == false)
            stateMachine.ChangeState(player.fallState);
        //2.5p和2.6p两处末尾修复bug使用，但此处未出现bug，使用上述代码显然会影响后续二段跳功能

        //输入PlayerInputSet界面填入的名称
        if (input.Player.Jump.WasPressedThisFrame())//WasPressedThisFrame是新输入系统PlayerInputSet脚本定义的
            stateMachine.ChangeState(player.jumpState);
        if (input.Player.普通攻击.WasPressedThisFrame())
            stateMachine.ChangeState(player.普通攻击状态);
        if (input.Player.弹反.WasPressedThisFrame())
            stateMachine.ChangeState(player.counterAttactState);
    }









}
