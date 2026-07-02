using UnityEngine;

public class Minotaur : Enemy
{
    [Header("牛头人专属攻击参数")]
    [cn("冲撞速度")] public float chargeSpeed = 12f;
    [cn("冲撞击退")] public Vector2 chargeKnockback;
    [cn("冲撞击退赋予速度持续时间")] public float chargeKnockbackTime = 0.2f;
    [cn("冲撞前与玩家最小距离")] public float chargeMinDistance = 3f;
    [cn("冲撞前与玩家最大距离")] public float chargeMaxDistance = 8f;
    [cn("冲撞命中距离")] public float chargeHitDistance = 1.2f;
    [cn("冲撞持续时间(秒)")] public float chargeDuration = 1.5f;
    [cn("冲撞冷却")] public float chargeCooldown = 4f;
    [cn("冲撞伤害倍率")] public float chargeDamageScale = 3f;

    [Header("撼地攻击参数")]
    [cn("撼地辅助线中心点")] public Transform stompStart;
    [cn("烟雾预制体")] public GameObject smokePrefab;
    [cn("撼地击退")] public Vector2 stompKnockback;
    [cn("撼地击退赋予速度持续时间")] public float stompKnockbackTime = 0.2f;
    [cn("撼地半径")] public float stompRadius = 2.5f;
    [cn("撼地冷却")] public float stompCooldown = 5f;
    [cn("撼地伤害倍率")] public float stompDamageScale = 1.2f;

    [Header("劈砍攻击参数")]
    [cn("劈砍击退")] public Vector2 slashKnockback;
    [cn("劈砍击退赋予速度持续时间")] public float slashKnockbackTime = 0.2f;
    [cn("劈砍冷却")] public float slashCooldown = 4f;
    [cn("劈砍伤害倍率")] public float slashDamageScale = 1.5f;

    [Header("横扫攻击参数")]
    [cn("横扫辅助线中心点")] public Transform spinStart;
    [cn("旋转击退")] public Vector2 spinKnockback;
    [cn("旋转击退赋予速度持续时间")] public float spinKnockbackTime = 0.2f;
    [cn("旋转横扫半径")] public float spinRadius = 2f;
    [cn("旋转横扫时接近玩家速度")] public float spinMoveSpeed = 2f;
    [cn("旋转横扫冷却")] public float spinCooldown = 6f;
    [cn("旋转横扫伤害倍率")] public float spinDamageScale = 0.8f;
    [cn("旋转横扫持续时间")] public float spinDuration = 1.5f;

    [Header("普通攻击参数")]
    [cn("普通攻击击退")] public Vector2 basicKnockback;
    [cn("普通攻击击退赋予速度持续时间")] public float basicKnockbackTime = 0.2f;
    [cn("普通攻击冷却")] public float basicCooldown = 1.5f;
    [cn("普通攻击伤害倍率")] public float basicDamageScale = 1f;

    // 攻击冷却计时器
    public float lastChargeTime { get; set; } = -999;
    public float lastStompTime { get; set; } = -999;
    public float lastSpinTime { get; set; } = -999;
    public float lastSlashTime { get; set; } = -999;
    public float lastBasicAttackTime { get; set; } = -999;

    // 状态实例
    public Minotaur_SlashState slashState;
    public Minotaur_BasicAttackState basicAttackState;
    public Minotaur_ChargePrepareState chargePrepareState;
    public Minotaur_ChargeState chargeState;
    public Minotaur_StompState stompState;
    public Minotaur_SpinState spinState;
    public Minotaur_BattleState minotaurBattleState;

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        slashState = new Minotaur_SlashState(this, stateMachine, "slash");
        basicAttackState = new Minotaur_BasicAttackState(this, stateMachine, "attack");

        chargePrepareState = new Minotaur_ChargePrepareState(this, stateMachine, "chargePrepare");
        chargeState = new Minotaur_ChargeState(this, stateMachine, "charge");
        stompState = new Minotaur_StompState(this, stateMachine, "stomp");
        spinState = new Minotaur_SpinState(this, stateMachine, "spin");
        minotaurBattleState = new Minotaur_BattleState(this, stateMachine, "battle");

        battleState = minotaurBattleState;

        haveDeadAnim = true;
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    public override void AfterDie()
    {
        boosHpBar?.gameObject.SetActive(false);
        DelayDestroyEnemy(2);
    }
    public void CreateSmoke()
    {
        Vector3 position = transform.position;
        position.y -= 0.84f;
        Instantiate(smokePrefab, position, Quaternion.identity);
    }
    public float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Mathf.Abs(player.position.x - transform.position.x);
    }

    public int DirectionToPlayer()
    {
        if (player == null) return 0;
        return player.position.x > transform.position.x ? 1 : -1;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); // 基类绘制索敌线、攻击距离线等

        // 冲撞范围：最小距离到最大距离的线段，加端点圆
        Gizmos.color = Color.magenta;
        Vector3 center = transform.position;
        Vector3 startPos = center + new Vector3(faceDirection * chargeMinDistance, 0.2f, 0);
        Vector3 endPos = center + new Vector3(faceDirection * chargeMaxDistance, 0.2f, 0);
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireSphere(startPos, 0.1f);
        Gizmos.DrawWireSphere(endPos, 0.1f);

        //冲撞触发伤害范围
        Gizmos.color = Color.red;
        Vector3 hitDirection = Vector3.right * faceDirection;
        Vector3 hitStart = center + new Vector3(0, 0.5f, 0);
        Vector3 hitEnd = hitStart + hitDirection * chargeHitDistance;
        Gizmos.DrawLine(hitStart, hitEnd);
        Gizmos.DrawWireSphere(hitEnd, 0.1f);

        // 跺脚半径：圆形轮廓
        Gizmos.color = new Color(1, 0.5f, 0); // 橙色
        Gizmos.DrawWireSphere(stompStart.position, stompRadius);

        // 旋转横扫半径：圆形轮廓 
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spinStart.position, spinRadius);
    }
}