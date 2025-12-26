using UnityEngine;

public class Player_CounterAttactState : PlayerState
{
    private 玩家战斗及目标检测 combat;
    private bool 是否已弹反;
    public Player_CounterAttactState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<玩家战斗及目标检测>();
    }
    public override void Enter()
    {
        base.Enter();

        stateTimer = combat.获得弹反动作僵直时间();
        是否已弹反 = combat.尝试弹反();//给其赋值为bool类型方法的返回值true/false
        //只执行一次尝试弹反()，也就是只检测一次碰撞体，如果没检测到碰撞体或碰撞体无弹反接口
        //就只会返回已弹反：false并被赋给是否已弹反
        anim.SetBool("弹反成功", 是否已弹反);
    }
    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, rb.linearVelocity.y);

        if (触发结束)
            stateMachine.ChangeState(player.idleState);
        if (stateTimer < 0 && 是否已弹反 == false)
            stateMachine.ChangeState(player.idleState);
        //当没能弹反成功时，便要等待僵直时间作为惩罚，时间结束退出至空闲状态
    }
}
