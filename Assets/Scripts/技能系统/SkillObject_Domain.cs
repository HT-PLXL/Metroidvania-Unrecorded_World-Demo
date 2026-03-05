using UnityEngine;

public class SkillObject_Domain : SkillObject_Base
{
    private Skill_Domain domainManager;
    private float expandSpeed = 2;//扩大速度
    private float slowDownPercent = 0.9f;
    private float duration;
    private Vector3 targetScale;
    private bool isShrinking;//缩小

    public void SetUpDomain(Skill_Domain domainManager)
    {
        this.domainManager = domainManager;
        float maxSize = domainManager.maxSize;
        duration = domainManager.GetDomainDuration();
        slowDownPercent = domainManager.GetSlowDownPercent();
        expandSpeed = domainManager.expandSpeed;
        targetScale = Vector3.one * maxSize;
        Invoke(nameof(ShrinkDomain), duration);
    }
    private void Update()
    {
        HandleScale();
    }
    private void HandleScale()//处理变大与缩小
    {
        float sizeDiffrence = Mathf.Abs(transform.localScale.x - targetScale.x);
        bool shouldChangeScale = sizeDiffrence > 0.1f;
        if (shouldChangeScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, expandSpeed * Time.deltaTime);
        if (isShrinking && sizeDiffrence < 0.1f)
        {
            domainManager.ClearTarget();
            Destroy(gameObject);
        }
    }
    private void ShrinkDomain()//缩小方法
    {
        targetScale = Vector3.zero;//一段延迟后调整期望缩放为0，启动缩小
        isShrinking = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isShrinking)//测试发现如果怪物速度很快，有可能会碰到正在缩小的领域，使怪物再次减速，所以增加判定
            return;
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null)
            return;
        domainManager.AddTarget(enemy);
        enemy.moveSpeed = 1;
        enemy.SlowDownEntity(duration, slowDownPercent, true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null)
            return;
        enemy.StopSlowDown();
    }
}
