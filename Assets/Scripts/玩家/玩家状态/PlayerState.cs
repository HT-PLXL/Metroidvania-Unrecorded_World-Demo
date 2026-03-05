using UnityEngine;

public abstract class PlayerState : EntityState
    //abstract变成抽象类，使得不能直接访问，相当于将其变成一个父类，在继承后，其子类可访问
{
    protected Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skillManager;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName):base(stateMachine,animBoolName)
        //构造函数，完成对象初始化
    {
        this.player = player;//同色对应，蓝色的是传入构造函数的参数

        anim = player.anim;//给父类和上面声明的空实例赋值
        rb = player.rb;
        input = player.input;
        entity_stats = player.entityStats;
        skillManager = player.skillManager;
    }
    public override void Update()
    {
        base.Update();


        if (input.Player.冲刺.WasPressedThisFrame() && CanDash())
        {
            skillManager.dash.setSkillOnCooldown();//进入冷却
            stateMachine.ChangeState(player.dashState);
        }
        if(input.Player.终极技能.WasPressedThisFrame() && skillManager.domain.CanUseSkill())//终极技能--领域
        {
            if (skillManager.domain.IsInstantDomain())
                skillManager.domain.CreateDomain();
            else
                stateMachine.ChangeState(player.domainState);
            skillManager.domain.setSkillOnCooldown();
        }
    }
    public override void UpdateAnimator()
    {
        base.UpdateAnimator();
        
        anim.SetFloat("yVelocity", rb.linearVelocity.y);//将y轴速度赋值给动画师界面的参数yVelocity，实现jump/fall动画切换
    }
    private bool CanDash()
    {
        if(skillManager.dash.CanUseSkill() == false)
            return false;
        if (player.isDetectWall)
            return false;
        if(stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainState)
            return false;
        return true;
    }







}
