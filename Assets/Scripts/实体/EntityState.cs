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
    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerOver = false;
    }
    //在进入该状态时执行特定操作，对应下面Exit离开状态时执行操作
    public virtual void Update()//注意实体状态是没有继承MonoBehaviour的，此处由其他脚本调用
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimator();//是virtual方法，供子类调用
    }
    public virtual void FixedUpdate()//注意实体状态是没有继承MonoBehaviour的，此处由其他脚本调用
    {

    }
    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
        //以currentState.Exit()的idlestate.Exit为例，它有3参，第3参animBoolName为实例idle                                 
        //此处执行anim.SetBool，字符串类型的参数集合“animBoolName”全部修改为false，因为只传了1个第3参，此时内部只有参数idle，修改为false                                 
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
        anim.SetFloat("attackSpeedMultiplier", attackSpeed);
    }
}
