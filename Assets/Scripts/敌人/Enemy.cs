using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyHp enemyHp {  get; private set; }

    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_StunnedState stunnedState;

    [Header("战斗细节")]
    public float battleMoveSpeed = 3;
    public float attackDistance = 2;
    public float battleTimeDuration = 5;
    public float 最小后撤距离 = 1;
    public Vector2 后撤速度;

    [Header("移动细节")]
    public float idleTime = 2;
    public float moveSpeed = 1.4f;
    [Range(0,2)]
    public float 移动与动画速率比 = 1;

    [Header("僵直细节")]
    public float stunnedDuration = 1;
    public Vector2 stunnedVelocity = new Vector2(7,7);
    [SerializeField] protected bool canBeStunned;

    [Header("detectPlayer")]
    [SerializeField] private LayerMask 什么是玩家;
    [SerializeField] private Transform 玩家检测起点;
    [SerializeField] private float 玩家检测距离;

    public Transform player { get; private set; }
    public float activeSlowDownmultiplier { get; private set; } = 1;
    protected override void Awake()
    {
        base.Awake();
        enemyHp = GetComponent<EnemyHp>();
    }
    public float GetMoveSpeed() => moveSpeed * activeSlowDownmultiplier;
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowDownmultiplier;

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)//冰冻减速异常效果实现
    {
        ////存储原始值
        //float originalMoveSpeed = moveSpeed;
        //float originalBattleSpeed = battleMoveSpeed;
        //float originalAnimSpeed = anim.speed;//动画速度

        activeSlowDownmultiplier = 1 - slowMultiplier;

        //moveSpeed = moveSpeed * speedMultiplier;
        //battleMoveSpeed = battleMoveSpeed * speedMultiplier;
        anim.speed = anim.speed * activeSlowDownmultiplier;
        
        yield return new WaitForSeconds(duration);
        StopSlowDown();
        ////回归原始值
        //moveSpeed = originalMoveSpeed;
        //battleMoveSpeed = originalBattleSpeed;
        //anim.speed = originalAnimSpeed;
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
    private void 处理玩家死亡()
    {
        stateMachine.ChangeState(idleState);
    }
    public void 受击进入战斗状态(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;
        this.player = player;
        stateMachine.ChangeState(battleState);
    }
    public Transform 获取玩家组件()
    {
        if (player == null)
            player = detectPlayer().transform;//在小怪正常检测到玩家时被调用执行，此时为空，获取玩家组件
        return player;//小怪受击进入战斗状态后被调用，然后返回“实体攻击目标检测”中获取并传入这里的this.player的player
    }

    public RaycastHit2D detectPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(玩家检测起点.position, Vector2.right * 面对方向, 玩家检测距离, 什么是玩家 | whatIsGround);
        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("玩家"))
            return default;
        return hit;
        // "|"表示按位或，检测到其中一个条件就执行下一句
        //ray是射线，检测的是最先，最近碰到的碰撞体，如果检测到地面层类别的墙壁，就只会返回地面层类别墙壁的碰撞体
        //RaycastHit2D类型返回值包含隐式布尔转换，检测到碰撞体为true，反之false，详见Enemy_GroundState中注释
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(玩家检测起点.position, new Vector3(玩家检测起点.position.x + (面对方向 * 玩家检测距离), 玩家检测起点.position.y , 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(玩家检测起点.position, new Vector3(玩家检测起点.position.x + (面对方向 * attackDistance), 玩家检测起点.position.y, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(玩家检测起点.position, new Vector3(玩家检测起点.position.x + (面对方向 * 最小后撤距离), 玩家检测起点.position.y, 0));
    }
    private void OnEnable()
    {
        Player.在玩家死亡之时 += 处理玩家死亡;//使用+=将右式方法注册到左式的静态事件
        //当左式静态事件被执行时，右式也会被执行
    }
    private void OnDisable()
    {
        Player.在玩家死亡之时 -= 处理玩家死亡;
    }
}
