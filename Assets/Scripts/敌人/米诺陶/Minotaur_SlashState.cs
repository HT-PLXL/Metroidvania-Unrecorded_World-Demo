using UnityEngine;

public class Minotaur_SlashState : EnemyState
{
    private Minotaur minotaur;
    private bool hasDamaged;

    public Minotaur_SlashState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
        minotaur = enemy as Minotaur;
    }

    public override void Enter()
    {
        base.Enter();
        SetAttackSpeed();
        hasDamaged = false;
        // 朝向玩家
        if (minotaur.player != null)
        {
            int dir = minotaur.DirectionToPlayer();
            if (dir != 0) minotaur.HandleFlip(dir);
        }
        enemy.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();
        if (triggerOver)
            stateMachine.ChangeState(minotaur.battleState);
    }

    // 动画事件调用
    public void PerformSlashDamage()
    {
        if (hasDamaged) return;
        hasDamaged = true;

        if (minotaur.player == null) return;

        float dist = Vector2.Distance(minotaur.transform.position, minotaur.player.position);
        if (dist > minotaur.attackDistance) return;

        EntityHp playerHp = minotaur.player.GetComponent<EntityHp>();
        if (playerHp != null)
        {
            playerHp.SetCustomKnockback(minotaur.slashKnockback, minotaur.slashKnockbackTime);
        }
        DamageScaleFactor scale = new DamageScaleFactor { physical = minotaur.slashDamageScale, elemental = 1f };
        minotaur.combat.PerformAttackOnTarget(minotaur.player, scale);
    }
}