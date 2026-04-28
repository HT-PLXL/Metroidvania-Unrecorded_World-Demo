using UnityEngine;

public class SkillObject_SpinSword : SkillObject_Sword
{
    private float maxFlyDistance;
    private float attackPerSecond;//每秒攻击次数
    private float attackTimer;
    private bool stopAlready;
    protected override void Update()
    {
        HandleAttack();
        HandleStop();
        HandleComeBack();
        if(stopAlready)
            Invoke(nameof(ShouldComeBack), throwSword.maxAttackDuration);
    }
    public override void SetUpSword(Skill_ThrowSword throwSword, Vector2 direction, bool isPhysicalDamage)
    {
        base.SetUpSword(throwSword, direction, isPhysicalDamage);

        anim?.SetTrigger("飞剑旋转");
        maxFlyDistance = throwSword.maxFlyDistance;
        attackPerSecond = throwSword.attackPerSecond;
        Invoke(nameof(ShouldComeBack),throwSword.maxDuration);
    }
    private void HandleStop()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance > maxFlyDistance&& rb.simulated == true)
            rb.simulated = false;
    }
    private void HandleAttack()
    {
        attackTimer-= Time.deltaTime;
        if (attackTimer < 0)
        {
            DamageEnemyInRadius(transform, checkRadius);
            attackTimer = 1 / attackPerSecond;
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        rb.simulated = false;
        stopAlready = true;
    }

}
