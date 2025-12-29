using UnityEngine;

public abstract class PlayerState : EntityState
    //abstract变成抽象类，使得不能直接访问，相当于将其变成一个父类，在继承后，其子类可访问
{
    protected Player player;
    protected PlayerInputSet input;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName):base(stateMachine,animBoolName)
        //构造函数，完成对象初始化
    {
        this.player = player;//同色对应，蓝色的是传入构造函数的参数

        anim = player.anim;//给父类和上面声明的空实例赋值
        rb = player.rb;
        input = player.input;
        entity_stats = player.entityStats;
    }
    public override void Update()
    {
        base.Update();


        if (input.Player.冲刺.WasPressedThisFrame() && 允许冲刺())
            stateMachine.ChangeState(player.冲刺状态);
    }
    public override void 更新动画师界面参数()
    {
        base.更新动画师界面参数();
        
        anim.SetFloat("yVelocity", rb.linearVelocity.y);//将y轴速度赋值给动画师界面的参数yVelocity，实现jump/fall动画切换
    }
    private bool 允许冲刺()
    {
        if (player.检测到墙壁)
            return false;
        if(stateMachine.currentState == player.冲刺状态)
            return false;
        return true;
    }







}
