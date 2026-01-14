using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField][cn("敌人")] protected LayerMask whoIsEneny;
    [SerializeField][cn("检测圆心")] protected Transform targetCheck;
    [SerializeField][cn("检测范围")] protected float checkRadius = 1;
    protected void DamageEnemyInRadius(Transform transform, float radius)
    {
        foreach (var target in EnemyAround(transform, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
                continue;
            damageable.造成伤害(1, 1, ElementType.None, transform);
        }
    }
    protected Transform SearchTarget()//寻找范围内最近目标
    {
        Transform target = null;
        float closestDistance =Mathf.Infinity;//设定最近距离
        foreach (var enemy in EnemyAround(transform, 10))
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
    protected Collider2D[] EnemyAround(Transform transform, float radius)//搜寻一定范围目标
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
