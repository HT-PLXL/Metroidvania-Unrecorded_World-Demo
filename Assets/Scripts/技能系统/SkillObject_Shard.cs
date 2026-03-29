using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    public event Action OnExplode;
    private Skill_Shard skillShard;
    [SerializeField][cn("时空碎片爆炸预制体")] private GameObject vfxPrefab;
    private Transform target;
    private float speed;
    private void Update()
    {
        if (target == null)
            return;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);//实现晶体平滑移动
    }
    public void MoveToClosestTarget(float speed, Transform newtarget = null)//被调用才会启动搜索目标，才会执行update内容
    {
        target = newtarget == null ? SearchClosestTarget() : newtarget;
        this.speed = speed;
    }
    public void SetUpShard(Skill_Shard shard)
    {
        this.skillShard = shard;
        playerStats = shard.player.stats;
        damageScale = shard.damageScale;
        float time = shard.GetDetonateTime();
        Invoke(nameof(Explode), time);//延迟time后执行，这是继承了MonoBehaviour的内置方法
    }
    public void SetUpShard(Skill_Shard shard, float detonationTime, bool canMove, float shardSpeed, Transform target = null)//重载
    {
        this.skillShard = shard;
        playerStats = shard.player.stats;
        damageScale = shard.damageScale;
        Invoke(nameof(Explode), detonationTime);//延迟detonationTime后执行，这是继承了MonoBehaviour的内置方法
        if (canMove)
            MoveToClosestTarget(shardSpeed,target);
    }
    public void Explode()//爆炸方法
    {
        DamageEnemyInRadius(transform, checkRadius);
        GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);//特效有自己的销毁机制及时间
        vfx.GetComponentInChildren<SpriteRenderer>().color = skillShard.player.vfx.GetHitVfxColor(elementType);//改变水晶爆炸粒子颜色
        OnExplode?.Invoke();//爆炸时检查是否有订阅的方法？(即进入冷却)，有则触发该方法
        Destroy(gameObject);//销毁碎片本身
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;
        Explode();
    }
}
