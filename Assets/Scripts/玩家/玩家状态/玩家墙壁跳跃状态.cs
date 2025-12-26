using UnityEngine;

public class 玩家墙壁跳跃状态 : PlayerState
{
    public 玩家墙壁跳跃状态(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    //重要，此处依旧有bug，偶现于贴右墙时左+空格，有概率原地跳一下，待解决
    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(player.墙壁跳跃.x * -player.面对方向, player.墙壁跳跃.y);
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        //if(player.检测到墙壁)
        //    stateMachine.ChangeState(player.墙壁滑动状态);

    }
}
