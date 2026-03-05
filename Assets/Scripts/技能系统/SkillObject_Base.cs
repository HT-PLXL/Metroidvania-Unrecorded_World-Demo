using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField][cn("技能打击特效")]private GameObject onHitVfxBySkill;
    [SerializeField][cn("敌人")] protected LayerMask whoIsEneny;
    [SerializeField][cn("检测圆心")] protected Transform targetCheck;
    [SerializeField][cn("检测范围")] protected float checkRadius = 1;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Entity_Stats playerStats;
    protected DamageScaleFactor damageScale;
    protected ElementType elementType;
    protected bool targetGotHit;
    protected Transform lastTarget;
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    protected void DamageEnemyInRadius(Transform transform, float radius)//伤害范围内所有敌人
    {
        foreach (var target in AroundEnemy(transform, radius))
        {
            IDamageable damageTarget = target.GetComponent<IDamageable>();
            if (damageTarget == null)
                continue;
            AttackData attackData = playerStats.GetAttackData(damageScale);
            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            targetGotHit =  damageTarget.TakeDamage(physicalDamage, elementalDamage, attackData.element, transform);//造成伤害，基于使用者的元素伤害
            if (attackData.element != ElementType.None)
                target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(attackData.element, attackData.effectData); //effectData包含技能伤害和时间等数据
            if (targetGotHit)
            {
                lastTarget= target.transform;
                Instantiate(onHitVfxBySkill, target.transform.position, Quaternion.identity);
            }
            elementType = attackData.element;//存储一份元素种类给子类用
        }
    }
    protected Transform SearchClosestTarget()//寻找范围内最近目标
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;//设定最近距离
        foreach (var enemy in AroundEnemy(transform, 10))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)//在半径10内搜索，目标距离小于最近距离则成为新的最近距离
            {
                target = enemy.transform;
                closestDistance = distance;
            }
        }
        return target;
    }
    protected Collider2D[] AroundEnemy(Transform transform, float radius)//搜寻一定范围目标
    {
        return Physics2D.OverlapCircleAll(transform.position, radius, whoIsEneny);//检测目标
    }
    protected virtual void OnDrawGizmos()//画线
    {
        if (targetCheck == null)
            targetCheck = transform;
        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
