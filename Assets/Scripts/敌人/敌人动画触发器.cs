using UnityEngine;

public class 敌人动画触发器 : 实体动画触发器
{
    private Enemy enemy;
    private 敌人视觉效果 enemyVFX;
    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
        enemyVFX = GetComponentInParent<敌人视觉效果>();
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
}
