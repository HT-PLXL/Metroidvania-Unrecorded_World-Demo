using UnityEngine;

public class EnemyAnimationTrigger : Entity_AnimationTrigger
{
    private Enemy enemy;
    private 敌人视觉效果 enemyVFX;
    private Enemy_BringerOfDeath bringerOfDeath;
    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
        enemyVFX = GetComponentInParent<敌人视觉效果>();
        bringerOfDeath = enemy as Enemy_BringerOfDeath;
    }
    private void EnableCounterWindow()
    {
        enemyVFX.可弹出攻击警告(true);
        enemy.EnableCounterWindow(true);
    }
    private void DisableCounterWindow()
    {
        enemyVFX.可弹出攻击警告(false);
        enemy.EnableCounterWindow(false);
    }
    private void SetStunRecoverT()//用于启用僵直恢复动画
    {
        enemy.stunRecovery = true;//设为true使僵直状态脚本延迟退出状态
        enemy.anim.SetBool("stunRecovery", enemy.stunRecovery);//启用僵直恢复动画
    }
    private void SetStunRecoverF()//用于关闭僵直恢复动画
    {
        enemy.stunRecovery = false;//退出状态
        enemy.anim.SetBool("stunRecovery", enemy.stunRecovery);//关闭僵直恢复动画
    }
    private void SpecialAttackTrigger()
    {
        enemy.SpecialAttack();
    }
    private void DestroyEnemy()
    {
        enemy.DelayDestroyEnemy(0);
    }
    private void Teleport()
    {
        bringerOfDeath.SetTeleportTrigger(true);
    }

}
