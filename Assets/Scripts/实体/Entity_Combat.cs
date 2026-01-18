using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX vfx;
    private Entity_Stats stats;//声明stats即可使用Entity_Stats中GetAttackData()
    [HideInInspector] public DamageScaleFactor damageScale;
    [Header("目标检测")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;
    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();

    }
    public void PerformAttack()//执行攻击
    {
        foreach (var target in GetDetectColliders())//对受击目标执行
        {
            IDamageable damageTarget = target.GetComponent<IDamageable>();
            if (damageTarget == null)
                continue;
            AttackData attackData = stats.GetAttackData(damageScale);

            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            //if (element != ElementType.None)
            //    target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(attackData.element, attackData.effectData);//使用攻击方的数据在受击方应用伤害与效果
            //暂时不让普攻使用元素伤害以及元素附着特效
            bool targetGotHit = damageTarget.TakeDamage(physicalDamage, elementalDamage, ElementType.None, transform);//暂时不让普攻使用元素伤害，此处传入ElementType.None
            if (targetGotHit)
                vfx.CreateHitVfx(target.transform, attackData.isCrit, attackData.element);//创建打击特效粒子
        }
    }
    protected Collider2D[] GetDetectColliders()//获得检测到的碰撞体
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
        //OverlapCircleAll存在可能的性能问题，后续再看需不需要优化吧。
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
