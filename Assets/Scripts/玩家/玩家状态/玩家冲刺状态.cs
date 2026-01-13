using UnityEngine;

public class 玩家冲刺状态 : PlayerState
{
    private float 原始重力;
    private int 冲刺方向;
    public 玩家冲刺状态(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        skillManager.dash.OnstartEffect();
        player.vfx.DoEchoEffect(player.dashDuration);//冲刺残影
        冲刺方向 = player.moveInput.x != 0 ? (((int)player.moveInput.x)) : player.面对方向;
        //使用冲刺方向而不是面对方向理论上能规避冲刺中可能出现的面对方向被改变导致冲刺变向的问题
        //右式能在进入冲刺状态时根据输入方向改变冲刺方向，否则只能先翻转再冲刺，影响手感

        stateTimer = player.dashDuration;

        原始重力 = rb.gravityScale;//把刚体重力原本的值赋给“原始重力”容器
        rb.gravityScale = 0;
        //重力设置为0，水平冲刺无下落
    }
    public override void Update()
    {
        base.Update();
        取消状态();

        player.SetVelocity(player.dashSpeed * 冲刺方向, 0);

        if (stateTimer < 0)
            if (player.检测到地面)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
    }
    public override void Exit()
    {
        base.Exit();

        skillManager.dash.OnEndEffect();

        player.SetVelocity(0, 0);
        rb.gravityScale = 原始重力;
    }
    private void 取消状态()
    {
        if (player.检测到墙壁)
        {
            if (player.检测到地面)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.墙壁滑动状态);
        }
    }
}
