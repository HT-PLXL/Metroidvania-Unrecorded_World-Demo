using UnityEngine;

public class TrapSpike : SpikeBase
{
    [Header("攻击配置")]
    [SerializeField] private float attackCooldown = 2f;       // 两次完整攻击之间的冷却
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float damageInterval = 0.3f;     // 同一次攻击中两次伤害的最小间隔

    private Animator animator;
    private bool canAttack = true;          // 是否允许触发新的攻击动作
    private float cooldownTimer;            // 外部攻击冷却倒计时
    private IDamageable currentPlayer;      // 当前在范围内的玩家
    private float lastDamageTime;           // 上次对玩家造成伤害的时间

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("TrapSpike 需要挂载 Animator 组件！");

        // 保证游戏开始时就能立刻造成第一次伤害
        lastDamageTime = -damageInterval;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayerLayer(other.gameObject.layer))
            return;

        if (other.TryGetComponent<IDamageable>(out var damageable))//获取实现伤害接口的脚本组件
        {
            currentPlayer = damageable;

            if (canAttack)
            {
                TriggerAttack();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayerLayer(other.gameObject.layer))
            return;

        if (other.TryGetComponent<IDamageable>(out var damageable) && damageable == currentPlayer)
        {
            currentPlayer = null;
        }
    }

    private void Update()
    {
        if (!canAttack)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canAttack = true;

                // 如果玩家还在范围内，自动发起下一次攻击
                if (currentPlayer != null)
                {
                    TriggerAttack();
                }
            }
        }
    }

    private void TriggerAttack()
    {
        canAttack = false;
        cooldownTimer = attackCooldown;
        animator.SetTrigger("attack");
    }

    /// <summary>
    /// 由动画事件调用（可在攻击动画的多个关键帧调用）
    /// </summary>
    public void ApplyDamageOnEvent()
    {
        // 必须满足三个条件：玩家在范围内、距上次伤害已过间隔
        if (currentPlayer != null && Time.time - lastDamageTime >= damageInterval)
        {
            lastDamageTime = Time.time;
            ApplyDamage(currentPlayer);
        }
    }

    private bool IsPlayerLayer(int layer)
    {
        return (playerLayer.value & (1 << layer)) != 0;
    }
}