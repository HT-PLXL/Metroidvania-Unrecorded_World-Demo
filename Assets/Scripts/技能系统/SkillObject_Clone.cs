using UnityEngine;
using System.Collections;
public class SkillObject_Clone : SkillObject_Base
{
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private GameObject DeathVfx;
    [SerializeField] private float trailMoveSpeed = 15;
    private Transform playerTransform;
    private Skill_Clone cloneManager;
    private TrailRenderer trail;
    private EntityHp playerHp;
    private SkillObject_Hp cloneHp;
    private Player_SkillManager skillManager;
    private Entity_StatusHandler playerStatusHandler;
    public int maxAttackCounts { get; private set; }

    public void SetUpClone(Skill_Clone cloneManager)
    {
        this.cloneManager = cloneManager;
        playerStats = cloneManager.player.stats;
        damageScale = cloneManager.damageScale;
        maxAttackCounts = cloneManager.GetMaxAttackCounts();
        playerTransform = cloneManager.transform.root;
        playerHp = cloneManager.player.hp;
        skillManager = cloneManager.skillManager;
        playerStatusHandler = cloneManager.player.statusHandler;
        flipClone();
        anim.SetBool("canAttack", maxAttackCounts > 0);
        cloneHp = GetComponent<SkillObject_Hp>();
        trail = GetComponentInChildren<TrailRenderer>();//获取轨迹组件并关闭
        trail.gameObject.SetActive(false);
        Invoke(nameof(HandleDeath), cloneManager.GetCloneDuration());
    }
    private void Update()
    {
        bool shouldMoveTrail = trail.gameObject.activeSelf;//activeSelf是表示自身是否激活的bool值，不受父对象影响
        if (shouldMoveTrail)
            HandleTrailMovement();
        else
        {
            anim.SetFloat("yVelocity", rb.linearVelocity.y);
            StopMovement();//阻止复制体自身横向移动
        }
    }
    private void flipClone()
    {
        Transform target = SearchClosestTarget();
        if (target != null && target.position.x < transform.position.x)
            transform.Rotate(0, 180, 0);
    }
    private void HandleTrailMovement()//处理灵魂粒子轨迹位置，此时复制体动画已关闭
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, trailMoveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
            StartCoroutine(TouchPlayerCo());
    }
    private IEnumerator TouchPlayerCo()
    {
        // 添加微小随机延迟（0-0.1秒），避免多个粒子同时触发
        yield return new WaitForSeconds(Random.Range(0f, 0.1f));
        HandleTouchPlayer();
        Destroy(gameObject);
    }



    private void HandleTouchPlayer()//灵魂粒子接触玩家后内容（回血/减CD/清除debuff）
    {
        float healAmount = cloneHp.lastDamage * cloneManager.GetDamagePercentHeal();//计算生命回复量
        playerHp.IncreaseHp(healAmount);

        float cooldownReduction = cloneManager.GetcooldownReduceSeconds();
        skillManager.ReduceAllSkillCooldown(cooldownReduction);

        if(cloneManager.CanRemoveDebuff())
            playerStatusHandler.RemoveAllDebuff();
    }
    public void HandleDeath()
    {
        Instantiate(DeathVfx, transform.position, Quaternion.identity);
        if (cloneManager.ShouldUseTrail())//判断是否应该启用灵魂粒子轨迹
        {
            anim.gameObject.SetActive(false);
            trail.gameObject.SetActive(true);
            rb.simulated =false;
        }
        else
            Destroy(gameObject);
    }
    public void PerformAttack()//动画执行
    {
        DamageEnemyInRadius(targetCheck, 1);
        if (targetGotHit == false)
            return;
        bool canClone = Random.value < cloneManager.GetCloneChance();
        float xOffset = transform.position.x < lastTarget.position.x ? 1 : -1;//设置一点偏移量
        if (canClone)
            cloneManager.CreateClone(lastTarget.position + new Vector3(xOffset, 0));
    }
    private void StopMovement()//阻止水平移动
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, WhatIsGround);
        if (hit2D.collider != null)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}
