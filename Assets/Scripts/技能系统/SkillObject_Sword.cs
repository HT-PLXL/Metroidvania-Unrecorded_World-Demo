using UnityEngine;

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_ThrowSword throwSword;
    protected float comeBackSpeed = 50;
    protected Transform playerTransform;
    protected float maxDistance = 25;
    protected bool shouldComeBack;
    protected bool alreadyStop;


    protected virtual void Update()
    {
        transform.right = rb.linearVelocity;//剑尖方向和transform.right重合，因此可以用速度方向改变剑方向
        HandleComeBack();
    }
    public virtual void SetUpSword(Skill_ThrowSword throwSword, Vector2 direction)//抛出剑的方法，同时接收Skill_ThrowSword的掷剑升级信息(可于技能容器修改)，并传递给技能实体脚本替换默认值
    {
        rb.linearVelocity = direction;//给一个初速度，就自动做斜抛运动了
        //用技能容器值替换默认值
        this.throwSword = throwSword;
        playerTransform = throwSword.transform.root;//实际上获取的是技能容器transform组件的根组件player.transform
        playerStats = throwSword.player.entityStats;
        damageScale = throwSword.damageScale;
    }
    protected void HandleComeBack()//判断超过最大距离，或设定应该返回时，平滑接近玩家并销毁
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if(distance > maxDistance)
            shouldComeBack = true;
        if (shouldComeBack == false)
            return;
        rb.simulated = false;
        if (transform.parent != null && alreadyStop)//用于对象碰撞体取消这类特殊情况，比如骷髅碰撞体取消后下坠会抵消返回速度
        {
            Collider2D parentCollider = transform.parent.GetComponent<Collider2D>();
            if (parentCollider != null && parentCollider.enabled ==false)
            {
                transform.parent = playerTransform;
                alreadyStop = false;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, comeBackSpeed * Time.deltaTime);//平滑移动接近玩家然后销毁
        if (distance < 0.5f)
            Destroy(gameObject);
    }
    public void ShouldComeBack()
    {
        shouldComeBack =true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        StopSword(collider);
        DamageEnemyInRadius(transform, checkRadius);
    }
    protected void StopSword(Collider2D collider)
    {
        rb.simulated = false;//停止物理模拟，这会立刻停止移动
        transform.parent = collider.transform;//把接触的碰撞体设为父对象，这样就会跟着它一起移动
        alreadyStop = true;
    }
}