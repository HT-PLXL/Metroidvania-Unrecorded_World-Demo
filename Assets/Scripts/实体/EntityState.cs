using UnityEngine;

public abstract class EntityState
    //abstrate变成抽象类
{
    protected StateMachine stateMachine;
    protected string animBoolName;
    protected Animator anim;//声明anim配合下面构造函数中anim进行缩写
    protected Rigidbody2D rb;//用于缩写
    protected Entity_Stats entity_stats;
    protected float stateTimer;//状态计时器
    protected bool triggerOver;

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }
    public virtual void Enter()//virtual：虚方法，允许子类用override进行重写，Enter返回类型为空
    {
        //——————————注意子类没有enter会自动执行父类的enter——————————

        anim.SetBool(animBoolName, true);
        triggerOver = false;
    }
    //在进入该状态时执行特定操作，对应下面Exit离开状态时执行操作
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;//右式是增量时间，结合相关代码就是从某个时间开始加减，可以为负
        UpdateAnimator();//virtual属性的方法被调用会执行其重写方法
    }
    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);//以currentState.Exit()的idlestate.Exit为例，它有3参，第3参animBoolName为实例idle
                                          //此处执行anim.SetBool，字符串类型的参数集合“animBoolName”全部修改为false，因为只传了1个第3参，此时内部只有参数idle，修改为false
                                          //注意上面声明的this.animBoolName = animBoolName;因此也可以理解括号中为（animBoolName = 实例参数animBoolName（即为idle），false）
    }
    public void AnimTrigger()
    {
        triggerOver = true;
    }
    public virtual void UpdateAnimator()//virtual属性的方法被调用会执行其重写方法
    {

    }
    public void SetAttackSpeed()
    {
        float attackSpeed = entity_stats.attack.attackSpeed.GetValue();
        anim.SetFloat("攻击速度乘数",attackSpeed);
    }
}
