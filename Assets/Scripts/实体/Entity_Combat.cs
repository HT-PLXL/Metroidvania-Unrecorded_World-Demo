using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public event Action<float> OnTakingPhysicalDamage;//触发事件时顺带传入一个值
    private Entity_VFX vfx;
    private Entity_SFX sfx;
    private Entity_Stats stats;//声明stats即可使用Entity_Stats中GetAttackData()
    [HideInInspector] public DamageScaleFactor basicDamageScale;
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
    public void PerformAttack()//执行攻击
    {
        bool anyTargetGotHit = false;
        foreach (var target in GetDetectColliders(whatIsTarget))//对受击目标执行
        {
            IDamageable damageTarget = target.GetComponent<IDamageable>();
            if (damageTarget == null)
                continue;
            AttackData attackData = stats.GetAttackData(basicDamageScale);

            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            //if (element != ElementType.None)
            //    target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(attackData.element, attackData.effectData);//使用攻击方的数据在受击方应用伤害与效果
            //暂时不让普攻使用元素伤害以及元素附着特效
            bool targetGotHit = damageTarget.TakeDamage(physicalDamage, elementalDamage, ElementType.None, transform);//暂时不让普攻使用元素伤害，此处传入ElementType.None
            if (targetGotHit)
            {
                anyTargetGotHit = true;
                OnTakingPhysicalDamage?.Invoke(physicalDamage);//触发事件时顺带传入物理伤害
                vfx.CreateHitVfx(target.transform, attackData.isCrit, attackData.element);//创建打击特效粒子
                sfx?.PlayAttackHit();
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

        bool targetGotHit = damageTarget.TakeDamage(physicalDamage, elementalDamage, ElementType.None, transform);//暂时不让普攻使用元素伤害，此处传入ElementType.None
        if (targetGotHit)
        {
            anyTargetGotHit = true;
            OnTakingPhysicalDamage?.Invoke(physicalDamage);//触发事件时顺带传入物理伤害
            vfx.CreateHitVfx(target.transform, attackData.isCrit, attackData.element);//创建打击特效粒子
            sfx?.PlayAttackHit();
        }

        if (anyTargetGotHit)
            sfx?.PlayAttackHit();
        else
            sfx?.PlayAttackMiss();
    }
    protected Collider2D[] GetDetectColliders(LayerMask whatNeedDetect)//获得检测到的碰撞体
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatNeedDetect);
        //OverlapCircleAll存在可能的性能问题，后续再看需不需要优化吧。
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
