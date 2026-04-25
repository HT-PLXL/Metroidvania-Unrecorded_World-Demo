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
        //player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);//实现移动
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        // 只需要设置玩家自己的期望速度，平台速度已由 Player类中的SetVelocity 自动叠加
        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);
    }

}
