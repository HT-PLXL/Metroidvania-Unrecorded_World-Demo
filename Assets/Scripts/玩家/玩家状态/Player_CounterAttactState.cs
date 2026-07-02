using UnityEngine;

public class Player_CounterAttactState : PlayerState
{
    private Player_Combat combat;
    private bool counterPerformed;
    private bool hasSetValue;
    private float Countertimer;
    private bool hasCountered;
    public Player_CounterAttactState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }
    public override void Enter()
    {
        base.Enter();
        Countertimer = combat.Countertimer;
        hasSetValue = false;
        hasCountered = false;
        counterPerformed = false;
        stateTimer = 999f;
    }
    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, rb.linearVelocity.y);

        Countertimer -= Time.deltaTime;

        if (hasCountered == false && Countertimer > 0)
        {
            counterPerformed = combat.tryCounter();
            if (counterPerformed)
            {
                anim.SetBool("弹反成功", true);
                hasCountered = true;
            }
        }

        if (triggerOver)
            stateMachine.ChangeState(player.idleState);

        if (Countertimer < 0 && hasSetValue == false && hasCountered == false)
        {
            stateTimer = combat.stunTimeAfterTryCounter;
            hasSetValue = true;
        }
        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
        //当没能弹反成功时，便要等待僵直时间作为惩罚，时间结束退出至空闲状态
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool("弹反成功", false);
    }
}
