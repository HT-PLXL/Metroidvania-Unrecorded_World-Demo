using UnityEngine;

public class Player_AttackState : PlayerState
{
    private float 攻击时速度计时器;
    private int 连招索引 = 1;
    private int 连招总数 = 3;
    private int 攻击方向;
    private float 上次攻击触发时间;
    private bool 连招衔接;
    public Player_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if(连招总数 != player.attackVelocity.Length)
            连招总数 = player.attackVelocity.Length;
        //限制/确定数组和连招总数是否一致，没啥用哈哈
    }
    public override void Enter()
    {
        base.Enter();
        连招衔接 = false;
        重置连招索引();
        SetAttackSpeed();

        攻击方向 = player.moveInput.x != 0 ? (((int)player.moveInput.x)): player.面对方向;

        anim.SetInteger("普通攻击索引", 连招索引);

        产生攻击时速度();
    }


    public override void Update()
    {
        base.Update();

        处理攻击时速度();

        if (input.Player.普通攻击.WasPressedThisFrame())
            连招衔接 = true;

        if (triggerOver)
            处理状态退出();
    }
    public override void Exit()
    {
        base.Exit();
        连招索引++;
        上次攻击触发时间 = Time.time;//记录当前时间
    }
    private void 处理状态退出()
    {
        if (连招衔接)
        {
            anim.SetBool(animBoolName, false);//设置当前帧动画师界面的参数为假
            player.延迟进入攻击状态();
        }
        else
            stateMachine.ChangeState(player.idleState);
    }
    private void 处理攻击时速度()
    {
        攻击时速度计时器 -= Time.deltaTime;
        if (攻击时速度计时器 < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }
    private void 产生攻击时速度()
    {
        Vector2 攻击时速度 = player.attackVelocity[连招索引 - 1];
        攻击时速度计时器 = player.攻击时速度持续时间;
        player.SetVelocity(攻击时速度.x * 攻击方向, 攻击时速度.y);
        //这里的攻击方向改变速度方向，使得进入攻击状态时获得一个翻转的机会（player.HandleFlip）
    }
    private void 重置连招索引()
    {
        if (连招索引 > 连招总数)
            连招索引 = 1;//动画师界面：参数-普通攻击索引从1开始
        if (Time.time > 上次攻击触发时间 + player.连招重置时间)
            连招索引 = 1;
    }
}
