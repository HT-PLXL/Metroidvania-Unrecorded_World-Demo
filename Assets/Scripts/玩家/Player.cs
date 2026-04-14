using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static Player instance;//在整个游戏流程中，类只需要一个实例时才适合设为静态实例
    public static event Action OnPlayerDeath;
    //static静态，后接方法，直接使用类名+方法（）调用，但只能访问静态的方法和属性，不能访问实例成员
    //static event：静态事件不需要创建类的对象就可以订阅（subscribe）或触发（invoke）
    //如果是static event，可以用+=或者-=将一个方法在事件上注册/移除
    public PlayerInputSet input { get; private set; }//这里的PlayerInputSet是新输入脚本

    //创建新状态流程：先在输入窗口设置按键，然后去动画窗口选择动画帧
    //再去动画师窗口设置触发和结束的需要的参数
    //创建状态脚本，在此声明公开的脚本类型+变量
    //最后在Awake（）中new参数后在新的状态脚本写继承，触发、结束和状态转换
    public UI ui {  get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public Player_VFX vfx { get; private set; }
    public EntityHp hp { get; private set; }
    public Entity_StatusHandler statusHandler { get; private set; }
    public Player_Combat playerCombat { get; private set; }
    public Bag_Player bag { get; private set; }
    public Player_Stats stats { get; private set; }

    //————————————状态————————————
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
    //————————————状态————————————
    [Header("攻击细节")]
    [cn("攻击速度")] public Vector2[] attackVelocity;
    [cn("跳跃攻击时速度")] public Vector2 jumpAttackVelocity;
    [cn("跳跃攻击持续时间")] public float attackSpeedDuration = 0.1f;
    [cn("连招重置时间")] public float comboResetTime = 1;
    private Coroutine comboQueueCo;//连招排队协程
    [Header("移动详情")]
    [cn("移动速度")] public float moveSpeed;
    [cn("跳跃力度")] public float jumpForce;
    [cn("蹬墙跳力度")] public Vector2 wallJumpForce;
    [cn("空中移动倍率")][Range(0, 1)] public float airMoveMultiplier = 0.7f;
    [cn("滑墙下降倍率")][Range(0, 1)] public float WallSlideMultiplier = 0.4f;
    [Space]
    [cn("冲刺持续时间")] public float dashDuration = .25f;
    [cn("冲刺速度")] public float dashSpeed = 20f;
    [Header("终极技能详情")]
    [cn("上升速度")] public float riseSpeed = 25;
    [cn("最大上升距离")] public float riseMaxDistance = 3;

    public Vector2 moveInput { get; private set; }
    //moveinput在unity PlayerInputSet界面被映射为整数值
    public Vector2 mousePosition { get; private set; }

    public bool canJumpTwo;

    protected override void Awake()
    {
        base.Awake();

        instance = this;
        ui = FindAnyObjectByType<UI>();//全局范围查找第一个类型为UI的对象
        input = new PlayerInputSet();
        skillManager = GetComponent<Player_SkillManager>();
        vfx = GetComponent<Player_VFX>();
        hp = GetComponent<EntityHp>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        playerCombat = GetComponent<Player_Combat>();
        bag = GetComponent<Bag_Player>();
        stats = GetComponent<Player_Stats>();

        ui.SetupControlUI(input,this);//初始化UI输入控制

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
        stateMachine.Initialize(idleState);

        // 加载血条开关状态
        bool hpBarEnabled = PlayerPrefs.GetInt("HpBarEnabled", 1) == 1;
        if (hp != null)
            hp.EnableHpBar(hpBarEnabled);
    }
    public void TeleportPlayer(Vector3 position) => transform.position = position;//传送玩家
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        //存储原始值开始
        float originalMoveSpeed = moveSpeed;
        float originalJumpSpeed = jumpForce;
        float originalanimSpeed = anim.speed;//动画速度
        Vector2 originalWallJump = wallJumpForce;
        Vector2 originalJumpAttackVelocity = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = new Vector2[attackVelocity.Length];//攻击速度是3段普攻的不同数据，存储在数组（引用类型，变量实际只是个指针）不能简单的赋值保存，所以用for循环拷贝原数组
        for (int i = 0; i < attackVelocity.Length; i++)
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

        for (int i = 0; i < attackVelocity.Length; i++)
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
        OnPlayerDeath?.Invoke();//用Invoke执行委托，
        stateMachine.ChangeState(deadState);
    }
    public void DelayEnterAttackState()
    {
        if (comboQueueCo != null)
            StopCoroutine(comboQueueCo);
        comboQueueCo = StartCoroutine(DelayEnterAttackStateCo());
        //左式接收的是下面的流程控制器类型的帧结束处理
    }
    private IEnumerator DelayEnterAttackStateCo()//在当前帧结束时才允许状态转换，留出足够时间
    {
        yield return new WaitForEndOfFrame();//生成延迟并返回——帧结束处理（一帧的最后一件事）
        stateMachine.ChangeState(attackState);//PlayerState中Enter会刷新bool值为真
    }
    public void TryInteract()//寻找最近可交互对象并交互
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        Collider2D[] objectsAround = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        foreach (var target in objectsAround)
        {
            IInteractable interactable = target.GetComponent<IInteractable>();
            if (interactable == null)
                continue;
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target.transform;
            }
        }
        if (closest == null)
            return;
        closest.GetComponent<IInteractable>().Interact();
    }
    private void OnEnable()//Onable在Awake后执行，用于启动输入系统
    {
        input.Enable();//启动输入系统
        input.Player.鼠标.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        input.Player.施法.performed += ctx => skillManager.shard.TryUseSkill();
        input.Player.施法.performed += ctx => skillManager.clone.TryUseSkill();//记得调整
        input.Player.快捷物品槽1.performed += ctx => bag.TryUseQuickItemInSlot(1);
        input.Player.快捷物品槽2.performed += ctx => bag.TryUseQuickItemInSlot(2);
        //input.Player.具体动作.按键 执行/按下/抬起 += ctx => 方法
        //.performed输入完成 / started按下 / canceled抬起 是InputAction类型的事件
        //ctx为InputAction.CallbackContext 的实例ctx，包含按键值，输入的设备，触发时间等
        //如果不需要使用ctx，可以用 _ 代替作为参数占位符

        // 完整的匿名函数写法 以施法为例
        //input.Player.施法.performed += delegate (InputAction.CallbackContext ctx)
        //{
        //    TryUseSkill(); // 要执行的逻辑
        //};
    }
    private void OnDisable()
    {
        input.Disable();//关闭输入系统
    }
}
