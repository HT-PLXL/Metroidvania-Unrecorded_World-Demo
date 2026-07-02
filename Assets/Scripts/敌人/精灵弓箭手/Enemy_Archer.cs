using UnityEngine;

public class Enemy_Archer : Enemy
{
    public bool CanBeCountered { get => canBeStunned; }
    public Archer_BattleState ArcherBattleState { get; set; }
    [Header("弓箭手细节")]
    [SerializeField][cn("箭矢预制体")] private GameObject arrowPrefab;
    [SerializeField][cn("发射点")] private Transform arrowStartPoint;
    [SerializeField][cn("箭矢速度")] private float arrowSpeed =10;
    [cn("箭矢强力击退")] public bool HearyHitback;//击退数值在实体血量的TakeDamage手动更改
    [cn("强力击退力度")] public Vector2 Knockback;
    [cn("击退僵直时间")] public float KnockbackTime;

    [Header("箭矢跟踪设置")]
    [SerializeField][cn("启用跟踪")] public bool enableTracking = false;
    [SerializeField][cn("跟踪模式")] private Arrow.TrackingMode trackingMode = Arrow.TrackingMode.DistanceThreshold;
    [SerializeField][cn("延迟时间(秒) 或 触发距离(米)")] private float trackingDelayOrDistance = 0.5f;
    [SerializeField][cn("跟踪速度(-1=保持原速)")] private float trackingSpeed = -1f;

    [Header("触发器区域攻击模式（仅追踪模式生效）")]
    [cn("启用触发器区域攻击")] public bool useTriggerZoneAttack = false;
    [cn("触发器碰撞体（矩形区域）")] public BoxCollider2D triggerZone;

    [Header("初始战斗状态功能")]
    [cn("初始直接进入战斗状态")] public bool startInBattleState = false;
    [cn("战斗状态永不超时（配合区域模式）")] public bool keepBattleForever = false;

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        ArcherBattleState = new Archer_BattleState(this, stateMachine, "battle");
        battleState = ArcherBattleState;

        haveDeadAnim = true;
    }
    protected override void Start()
    {
        base.Start();

        //根据开关决定初始状态
        if (startInBattleState)
        {
            //直接进入战斗状态
            player = Player.instance?.transform;
            stateMachine.Initialize(battleState);
        }
        else
        {
            stateMachine.Initialize(idleState);
        }
    }
    public void HandleCounter()//实现接口
    {
        stateMachine.ChangeState(stunnedState);
    }
    public override void SpecialAttack()//射箭
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowStartPoint.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().SetupArrow(arrowSpeed * faceDirection,combat,Knockback,KnockbackTime, enableTracking, trackingMode, trackingDelayOrDistance, trackingSpeed);
        sfx?.PlaySpecialAttack();//播放远程攻击音效
        Destroy(arrow, 8);
    }
}
