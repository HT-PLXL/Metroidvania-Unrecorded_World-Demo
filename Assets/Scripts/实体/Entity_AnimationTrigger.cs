using UnityEngine;

public class Entity_AnimationTrigger : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entity_Combat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entity_Combat = GetComponentInParent<Entity_Combat>();
    }
    public void CurrentStateAnimTrigger()//动画的触发器
    {
        entity.CurrentStateAnimTrigger();
    }
    //动画的事件调用上述方法——再调用Entity.CurrentStateAnimTrigger——再调用EntityState动画事件触发
    //最后的方法内：triggerOver = true;

    private void attackTrigger()
    {
        entity_Combat.PerformAttack();
    }
}
