using UnityEngine;

public abstract class SpikeBase : MonoBehaviour
{
    [Header("伤害配置")]
    [SerializeField] protected float damage = 20f;            // 基础伤害
    [SerializeField] protected float elementalDamage = 0f;   // 元素伤害
    [SerializeField] protected ElementType element = ElementType.None; // 伤害元素类型

    // 对目标施加伤害，可被子类重写（如即死地刺）
    protected virtual void ApplyDamage(IDamageable target)
    {
        target.TakeDamage(damage, elementalDamage, element, transform);
    }

    // 子类根据自身逻辑在合适时机调用此方法
    protected void TryDamage(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            ApplyDamage(damageable);
        }
    }
}