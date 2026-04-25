using System.Collections;
using UnityEngine;

public class StoneHp : EntityHp
{
    [Header("——石巨人专属——")]
    [cn("转阶段时回血间隔")] public float turnStageRegenInterval;
    private Coroutine co;
    private bool canTurnSatge;
    private bool isRegenerating;

    private Stone stone => GetComponent<Stone>();
    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageSource)
    {
        if (stone.active)
        {
            OnHpUpdate += stone.boosHpBar.UpdateBOOSHpbar;
            bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageSource);//执行父方法，并获取是否被击中，用于判断
            //stone.boosHpBar.UpdateBOOSHpbar();

            if (wasHit == false)
                return false;//返回值用于实体战斗中判定是否启用攻击特效
        }

        if (stone.active == false && damageSource.GetComponent<Player>() != null)//确认组件中包含Player脚本，有则说明它是玩家
        {
            stone.player = damageSource;
            stone.active = true;
            stone.anim.SetBool("active", true);
            return false;
        }

        return true;//返回值用于实体战斗中判定是否启用攻击特效
    }

    private void Update()
    {
        if (stone.stage == 1 && GetHpPercent() < 0.33)
        {
            stone.isTurning = true;
            CanTakeDamage(false);//由动画事件解除
            RegenHpToMax();
            stone.stage = 2;
            stone.anim.SetInteger("stage", 2);
        }
        else if (stone.stage == 2 && GetHpPercent() < 0.15 && canTurnSatge)
        {
            stone.isTurning = true;
            CanTakeDamage(false);//由动画事件解除
            RegenHpToMax();
            stone.stage = 3;
            stone.anim.SetInteger("stage", 3);
        }

    }
    private void RegenHpToMax()
    {
        if (isRegenerating)
            return;
        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(GetIncreaseHp());
    }
    private IEnumerator GetIncreaseHp()
    {
        isRegenerating = true;
        if (currentHp < 0)
            currentHp = 0;
        while (currentHp < entityStats.GetMaxHp())
        {
            IncreaseHp(entityStats.GetMaxHp() / 20);
            yield return new WaitForSeconds(turnStageRegenInterval);
        }
        canTurnSatge = true;
        isRegenerating = false;
    }
    protected override void Die()
    {
        if (canTakeDamage == true && stone.stage == 3)
            base.Die();
    }
}
