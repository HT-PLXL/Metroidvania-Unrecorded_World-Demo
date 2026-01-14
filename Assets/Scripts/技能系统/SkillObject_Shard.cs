using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    [SerializeField][cn("时空碎片爆炸预制体")] private GameObject vfxPrefab;
    private Transform target;
    private float speed;
    private void Update()
    {
        if (target == null)
            return;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
    public void MoveToClosestTarget(float speed)//被调用才会启动搜索目标，才会执行update内容
    {
        target = SearchTarget();
        this.speed = speed;
    }
    public void SetUpShard(float time)
    {
        Invoke(nameof(Explode), time);//延迟time后执行
    }
    private void Explode()//爆炸
    {
        DamageEnemyInRadius(transform, checkRadius);
        Instantiate(vfxPrefab, transform.position, Quaternion.identity);//特效有自己的销毁机制及时间
        Destroy(gameObject);//销毁碎片本身
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;
        Explode();
    }
}
