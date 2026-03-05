using UnityEngine;

public class Player_CounterAttactState : PlayerState
{
    private Player_Combat combat;
    private bool 是否已弹反;
    public Player_CounterAttactState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }
    public override void Enter()
    {
        base.Enter();

        stateTimer = combat.GetStunTimeAfterTryCounter();
        是否已弹反 = combat.tryCounter();//给其赋值为bool类型方法的返回值true/false
        //只执行一次尝试弹反()，也就是只检测一次碰撞体，如果没检测到碰撞体或碰撞体无弹反接口
        //就只会返回已弹反：false并被赋给是否已弹反
        anim.SetBool("弹反成功", 是否已弹反);
    }
    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, rb.linearVelocity.y);

        if (triggerOver)
            stateMachine.ChangeState(player.idleState);
        if (stateTimer < 0 && 是否已弹反 == false)
            stateMachine.ChangeState(player.idleState);
        //当没能弹反成功时，便要等待僵直时间作为惩罚，时间结束退出至空闲状态
    }
}
