using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField][cn("µ–»À")] protected LayerMask whoIsEneny;
    [SerializeField][cn("ºÏ≤‚‘≤–ƒ")] protected Transform targetCheck;
    [SerializeField][cn("ºÏ≤‚∑∂Œß")] protected float checkRadius = 1;
    protected void DamageEnemyInRadius(Transform transform, float radius)
    {
        foreach (var target in EnemyAround(transform, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
                continue;
            damageable.‘Ï≥……À∫¶(1, 1, ElementType.None, transform);
        }
    }
    protected Collider2D[] EnemyAround(Transform transform, float radius)
    {
        return Physics2D.OverlapCircleAll(transform.position, radius, whoIsEneny);//ºÏ≤‚ƒø±Í
    }
    protected virtual void OnDrawGizmos()//ª≠œﬂ
    {
        if (targetCheck == null)
            targetCheck = transform;
        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
