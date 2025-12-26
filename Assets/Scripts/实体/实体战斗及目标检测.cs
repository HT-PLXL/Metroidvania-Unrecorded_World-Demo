using Unity.VisualScripting;
using UnityEngine;

public class 实体战斗及目标检测 : MonoBehaviour
{
    private 实体视觉效果 vfx;
    private Entity_stats stats;

    [Header("目标检测")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("异常状态效果处理")]
    [SerializeField] private float defaultDuration = 3;
    [SerializeField] private float chillSlowMultiplier = .4f;

    private void Awake()
    {
        vfx = GetComponent<实体视觉效果>();
        stats = GetComponent<Entity_stats>();

    }
    public void 执行攻击()
    {
        foreach(var target in 获得检测的碰撞体())//对攻击目标执行
        {
            //实体血量 targetHp = target.GetComponent<实体血量>();
            //targetHp ? .造成伤害(damage,transform);//传入伤害以及受伤对象的组件

            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue;
            float elementalDamage = stats.GetElementalDamage(out ElementType element);
            float damage = stats.GetPhyiscalDamage(out bool isCrit);//这里的isCrit可以是任何名字，只是原函数中out传过来的数值的容器

            bool targetGotHit = damageable.造成伤害(damage, elementalDamage,element, transform);

            if (element != ElementType.None)
                ApplyStatusEffect(target.transform, element);

            if (targetGotHit)
            {
                vfx.UpdataOnHitColor(element);
                vfx.启用攻击特效(target.transform, isCrit);
            }
        }
    }
    public void ApplyStatusEffect(Transform target, ElementType element)
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
        if (statusHandler == null)
            return;

        if (element == ElementType.Ice && statusHandler.CanApply(ElementType.Ice))
            statusHandler.ApplyChilledEffect(defaultDuration, chillSlowMultiplier);
    }
    protected Collider2D[] 获得检测的碰撞体()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
        //OverlapCircleAll存在可能的性能问题，后续再看需不需要优化吧。
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
