using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public event Action<float> OnTakingPhysicalDamage;//触发事件时顺带传入一个值
    private Entity_VFX vfx;
    private Entity_SFX sfx;
    private Entity_Stats stats;//声明stats即可使用Entity_Stats中GetAttackData()
    public DamageScaleFactor basicDamageScale;
    [Header("目标检测")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;
    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        sfx = GetComponent<Entity_SFX>();
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        bool anyTargetGotHit = false;
        foreach (var target in GetDetectColliders(whatIsTarget))
        {
            IDamageable damageTarget = target.GetComponent<IDamageable>();
            if (damageTarget == null)
                continue;

            AttackData attackData = stats.GetAttackData(basicDamageScale);
            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element; // 由武器元素伤害决定的属性

            bool targetGotHit = damageTarget.TakeDamage(physicalDamage, elementalDamage, element, transform, attackData.isCrit);

            if (targetGotHit)
            {
                anyTargetGotHit = true;
                OnTakingPhysicalDamage?.Invoke(physicalDamage);

                // 施加元素异常状态（若元素有效）
                if (element != ElementType.None)
                {
                    Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
                    if (statusHandler != null)
                        statusHandler.ApplyStatusEffect(element, attackData.effectData);
                }

                vfx.CreateHitVfx(target.transform, attackData.isCrit, attackData.element);
            }
        }
        if (anyTargetGotHit)
            sfx?.PlayAttackHit();
        else
            sfx?.PlayAttackMiss();
    }

    public void PerformAttackOnTarget(Transform target, DamageScaleFactor damageScaleFactor = null)
    {
        bool anyTargetGotHit = false;

        IDamageable damageTarget = target.GetComponent<IDamageable>();
        if (damageTarget == null)
            return;

        DamageScaleFactor damageScale = damageScaleFactor == null ? basicDamageScale : damageScaleFactor;
        AttackData attackData = stats.GetAttackData(damageScale);

        float physicalDamage = attackData.physicalDamage;
        float elementalDamage = attackData.elementalDamage;
        ElementType element = attackData.element;

        bool targetGotHit = damageTarget.TakeDamage(physicalDamage, elementalDamage, element, transform, attackData.isCrit);

        if (targetGotHit)
        {
            anyTargetGotHit = true;
            OnTakingPhysicalDamage?.Invoke(physicalDamage);

            if (element != ElementType.None)
            {
                Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
                if (statusHandler != null)
                    statusHandler.ApplyStatusEffect(element, attackData.effectData);
            }

            vfx.CreateHitVfx(target.transform, attackData.isCrit, attackData.element);
        }

        if (anyTargetGotHit)
            sfx?.PlayAttackHit();
        else
            sfx?.PlayAttackMiss();
    }
    public void CounterDamage(Collider2D target)//弹反造成伤害的方法
    {
        DamageScaleFactor damageScale = new DamageScaleFactor{physical = 1.5f};//1.5倍
        AttackData attackData = stats.GetAttackData(damageScale);

        float physicalDamage = attackData.physicalDamage;
        target.GetComponent<EntityHp>()?.ReduceHp(physicalDamage);
        sfx?.PlaySpecialAttack();
    }
    protected Collider2D[] GetDetectColliders(LayerMask whatNeedDetect)//获得检测到的碰撞体
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatNeedDetect);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
