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
    //entity被声明后是个空实例，需要获取参数，如果为Null会报错，因为此时unity不知道“entity.方法”具体是什么
    //其他脚本可以直接使用xxx.方法是因为构造函数传入了相关参数，完成了初始化，而这里没有
    //Awake（）是unity生命周期的第一个函数
    //在这里获取组件可以确保在大部分游戏逻辑
    //比如动画事件的触发之前就获取到里面的数据，比如Entity脚本定义的各种方法

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
