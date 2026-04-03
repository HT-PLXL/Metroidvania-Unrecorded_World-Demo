using UnityEngine;

public class Player_JumpAttackState : PlayerState
{
    private bool hadPerformed;//限制只执行一次
    public Player_JumpAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        hadPerformed = false;
        player.SetVelocity(player.jumpAttackVelocity.x * player.faceDirection, player.jumpAttackVelocity.y);
    }
    public override void Update()
    {
        base.Update();
        if (player.isDetectGround && hadPerformed == false)
        {
            hadPerformed = true;
            anim.SetTrigger("跳跃斩击触发器");//激活并进入“跳跃斩击结束”动画
            player.SetVelocity(0, rb.linearVelocity.y);
        }
        if (triggerOver && player.isDetectGround)//动画节点触发后“triggerOver”为true
            stateMachine.ChangeState(player.idleState);
    }
}
//————————————————————重要————————————————————
//进入跳跃斩击状态后，animBoolName：跳跃斩击 一直处于true状态，直至退出该状态
//检测到地面后trigger参数跳跃斩击触发器激活，动画师界面检测到该参数进入“跳跃斩击结束动画”
//但在“跳跃斩击结束”动画期间，上述if代码依旧在检测地面，然后激活“跳跃斩击触发器”
//这会导致下次进入跳跃斩击状态时，直接进入“跳跃斩击结束”
//因此声明bool参数“hadPerformed”并加入if中的判断,判断成功后改为true
//从而限制"跳跃斩击触发器"只激活一次