using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static event Action 在玩家死亡之时;
    //static静态，后接方法，直接使用类名+方法（）调用，但只能访问静态的方法和属性，不能访问实例成员
    //static event：静态事件不需要创建类的对象就可以订阅（subscribe）或触发（invoke）
    //如果是static event，可以用+=或者-=将一个方法在事件上注册/移除
    public PlayerInputSet input { get; private set; }//这里的PlayerInputSet是新输入脚本

    //创建新状态流程：先在输入窗口设置按键，然后去动画窗口选择动画帧
    //再去动画师窗口设置触发和结束的需要的参数
    //创建状态脚本，在此声明公开的脚本类型+变量
    //最后在Awake（）中new参数后创建新的状态脚本写继承，触发、结束和状态转换
    private UI ui;
    public Player_SkillManager skillManager {  get; private set; }
    public Player_VFX vfx { get; private set;}
    public EntityHp Hp { get; private set;}
    public Entity_StatusHandler statusHandler { get; private set; }


    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_AttackState attackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_CounterAttactState counterAttactState { get; private set; }
    public Player_ThrowSwordState throwSwordState { get; private set; }
    public Player_DomainState domainState { get; private set; }

    [Header("攻击细节")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float 攻击时速度持续时间 = 0.1f;
    public float 连招重置时间 = 1;
    private Coroutine 攻击排队;

    [Header("移动详情")]
    public float moveSpeed;
    public float jumpForce;
    public Vector2 wallJumpForce;
    [Range(0, 1)]//滑动条
    public float 空中移动倍率 = 0.7f;
    [Range(0, 1)]
    public float 滑墙下降倍率 = 0.7f;
    [Space]
    public float dashDuration = .25f;
    public float dashSpeed = 20f;
    [Header("终极技能详情")]
    public float riseSpeed = 25;
    public float riseMaxDistance = 3;

    public Vector2 moveInput { get; private set; }
    //moveinput在unity PlayerInputSet界面被映射为整数值
    public Vector2 mousePosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        ui = FindAnyObjectByType<UI>();//全局范围查找第一个类型为UI的对象
        input = new PlayerInputSet();
        skillManager = GetComponent<Player_SkillManager>();
        vfx = GetComponent<Player_VFX>();
        Hp =GetComponent<EntityHp>();
        statusHandler = GetComponent<Entity_StatusHandler>();

        idleState = new Player_IdleState(this, stateMachine, "idle");//橙色内容是动画师界面的参数，必须一致
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        counterAttactState = new Player_CounterAttactState(this, stateMachine, "弹反");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "墙壁滑动");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "冲刺");
        attackState = new Player_AttackState(this, stateMachine, "普通攻击");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "跳跃斩击");
        throwSwordState = new Player_ThrowSwordState(this, stateMachine, "throwSword");
        domainState = new Player_DomainState(this, stateMachine, "jumpFall");
        //重中之重，此处通过new将参数传递给各状态脚本并执行其构造函数
        //再通过他们的构造函数中的“：base”传递参数给他们的父类PlayerState
        //于是各子类状态脚本便可以通过父类中声明的stateMachine执行操作,如调用状态机中的公开方法
        //如果没有写显式的构造函数，会生成一个隐式的运行，如此处的StateMachine

    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initalize(idleState);
    }
    public void TeleportPlayer(Vector3 position)=> transform.position = position;//传送玩家
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        //存储原始值开始
        float originalMoveSpeed = moveSpeed;
        float originalJumpSpeed = jumpForce;
        float originalanimSpeed = anim.speed;//动画速度
        Vector2 originalWallJump = wallJumpForce;
        Vector2 originalJumpAttackVelocity = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = new Vector2[attackVelocity.Length];//攻击速度是3段普攻的不同数据，存储在数组（引用类型，变量实际只是个指针）不能简单的赋值保存，所以用for循环拷贝原数组
        for(int i = 0; i < attackVelocity.Length; i++)
        {
            originalAttackVelocity[i] = attackVelocity[i];
        }
        //存储原始值结束
        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed = moveSpeed * speedMultiplier;
        jumpForce = jumpForce * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;
        wallJumpForce = wallJumpForce * speedMultiplier;
        jumpAttackVelocity = jumpAttackVelocity * speedMultiplier;

        for(int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = attackVelocity[i] * speedMultiplier;
        }

        yield return new WaitForSeconds(duration);
        //回归原始值
        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpSpeed;
        anim.speed = originalanimSpeed;
        wallJumpForce = originalWallJump;
        jumpAttackVelocity = originalJumpAttackVelocity;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }
    public override void EntityDead()
    {
        base.EntityDead();
        在玩家死亡之时?.Invoke ();//用Invoke执行委托，
        stateMachine.ChangeState(deadState);
    }
    public void 延迟进入攻击状态()
    {
        if (攻击排队 != null)
            StopCoroutine(攻击排队);
        攻击排队 = StartCoroutine(延迟进入攻击状态协同程序());
        //左式接收的是下面的流程控制器类型的帧结束处理
    }
    private IEnumerator 延迟进入攻击状态协同程序()//IE在unity中被当做执行流程控制器
    {
        yield return new WaitForEndOfFrame();//生成延迟并返回——帧结束处理（一帧的最后一件事）
        stateMachine.ChangeState(attackState);//PlayerState中Enter会刷新bool值为真
    }
    private void OnEnable()//Onable在Awake后执行，用于启动输入系统
    {
        input.Enable();
        input.Player.鼠标.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        //实例.访问Player的Movement模块
        //.performed += ctx => ...为输入动作的 “performed” 事件注册回调函数
        //当输入被执行（即检测到有效输入）时触发。
        //对应还有started和canceled事件，对应按下（开始）和抬起（停止）
        //ctx为context，前后需保持一致

        input.Player.开关技能树.performed += ctx => ui.ToggleSkillTreeUI();
        input.Player.关闭技能树.performed += ctx => ui.CloseSkillTreeUI();
        input.Player.施法.performed += ctx => skillManager.shard.TryUseSkill();
        input.Player.施法.performed += ctx => skillManager.clone.TryUseSkill();//记得调整
    }
    private void OnDisable()
    {
        input.Disable();
    }



}
