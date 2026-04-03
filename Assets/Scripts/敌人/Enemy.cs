using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public Entity_Stats stats { get; private set; }
    public EnemyHp enemyHp { get; private set; }
    public Entity_Combat combat { get; private set; }
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_StunnedState stunnedState;

    [Header("战斗细节")]
    [cn("战斗移动速度")] public float battleMoveSpeed = 3;
    [cn("攻击距离-蓝线")] public float attackDistance = 2;
    [cn("战斗状态持续时间")] public float battleTimeDuration = 5;
    [cn("最小应该后撤距离-青线")] public float minBackJumpDistance = 1;
    [cn("后撤力度")] public Vector2 backJumpSpeed;
    [cn("攻击冷却")] public float attackCooldown;
    [cn("是否应该追逐玩家？")] public bool ShouldChasePlayer = true;


    [Header("检测玩家")]
    [SerializeField][cn("什么是玩家")] private LayerMask whoIsPlayer;
    [SerializeField][cn("检测玩家起点")] private Transform detectPlayerStart;
    [SerializeField][cn("检测玩家距离-黄线")] private float detectPlayerDistance;

    [Header("移动细节")]
    [cn("空闲时间")] public float idleTime = 2;
    [cn("移动速度")] public float moveSpeed = 1.4f;
    [cn("移动与动画速率比")][Range(0, 2)] public float moveAndAnimSpeedRatio = 1;

    [Header("僵直细节")]
    [cn("僵直时间")] public float stunnedDuration = 1;
    [cn("被弹反击飞力度")] public Vector2 stunnedVelocity = new Vector2(7, 7);
    [SerializeField][cn("是否可被弹反")] protected bool canBeStunned;
    [HideInInspector] public bool stunRecovery;//僵直恢复

    public bool haveDeadAnim { get; set; }
    public Transform player { get; private set; }
    public float activeSlowDownmultiplier { get; private set; } = 1;
    protected override void Awake()
    {
        base.Awake();
        enemyHp = GetComponent<EnemyHp>();
        stats = GetComponent<Entity_Stats>();
        combat = GetComponent<Entity_Combat>();
    }
    public void CanBeSelect(bool canBeSelect)
    {
        if (canBeSelect == false)
            gameObject.layer = LayerMask.NameToLayer("不可选中");
        else
            gameObject.layer = LayerMask.NameToLayer("敌人");
    }
    public virtual void SpecialAttack()
    {

    }
    public float GetMoveSpeed() => moveSpeed * activeSlowDownmultiplier;
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowDownmultiplier;

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)//冰冻减速异常效果实现
    {
        activeSlowDownmultiplier = 1 - slowMultiplier;
        anim.speed = anim.speed * activeSlowDownmultiplier;

        yield return new WaitForSeconds(duration);
        StopSlowDown();
    }
    public override void StopSlowDown()
    {
        activeSlowDownmultiplier = 1;
        anim.speed = 1;
        base.StopSlowDown();
    }
    public void EnableCounterWindow(bool enable)
    {
        canBeStunned = enable;
    }
    public override void EntityDead()
    {
        base.EntityDead();

        stateMachine.ChangeState(deadState);
    }
    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }
    public void TryEnterCombatState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;
        this.player = player;
        stateMachine.ChangeState(battleState);
    }
    public virtual void AfterDie()//死亡状态需要额外做的事
    {

    }
    public void DelayDestroyEnemy(float delay = 8)
    {
        Destroy(gameObject, delay);//延迟后销毁
    }
    public Transform GetPlayerTransform()
    {
        if (player == null)
            player = DetectPlayer().transform;//在小怪正常检测到玩家时被调用执行，此时为空，GetPlayerTransform
        return player;//小怪受击进入战斗状态后被调用，然后返回“实体攻击目标检测”中获取并传入这里的this.player的player
    }

    public RaycastHit2D DetectPlayer()
    {
        RaycastHit2D target = Physics2D.Raycast(detectPlayerStart.position, Vector2.right * faceDirection, detectPlayerDistance, whoIsPlayer | whatIsGround);
        if (target.collider == null || target.collider.gameObject.layer != LayerMask.NameToLayer("玩家"))
            return default;
        return target;
        // "|"表示按位或，检测到其中一个条件就执行下一句
        //ray是射线，检测的是最先，最近碰到的碰撞体，如果检测到地面层类别的墙壁，就只会返回地面层类别墙壁的碰撞体
        //RaycastHit2D类型返回值包含隐式布尔转换，检测到碰撞体为true，反之false，详见Enemy_GroundState中注释
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;//索敌玩家距离
        Gizmos.DrawLine(detectPlayerStart.position, new Vector3(detectPlayerStart.position.x + (faceDirection * detectPlayerDistance), detectPlayerStart.position.y, 0));
        Gizmos.color = Color.blue;//攻击距离
        Gizmos.DrawLine(detectPlayerStart.position - new Vector3(0, 0.1f), new Vector3(detectPlayerStart.position.x + (faceDirection * attackDistance), detectPlayerStart.position.y, 0) - new Vector3(0, 0.1f));
        Gizmos.color = Color.cyan;//最小应该后撤距离
        Gizmos.DrawLine(detectPlayerStart.position - new Vector3(0, 0.2f), new Vector3(detectPlayerStart.position.x + (faceDirection * minBackJumpDistance), detectPlayerStart.position.y, 0) - new Vector3(0, 0.2f));
    }
    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;//使用+=将右式方法注册到左式的静态事件
        //当左式静态事件被执行时，右式也会被执行
    }
    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }
}
