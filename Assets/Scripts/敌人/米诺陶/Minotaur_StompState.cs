using UnityEngine;

public class Minotaur_StompState : EnemyState
{
    private Minotaur minotaur;
    private bool hasDamaged;

    public Minotaur_StompState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
        minotaur = enemy as Minotaur;
    }

    public override void Enter()
    {
        base.Enter();
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
    public void PerformStompDamage()
    {
        if (hasDamaged) return;

        if (minotaur.player == null) return;

        float dist = Vector2.Distance(minotaur.transform.position, minotaur.player.position);
        if (dist > minotaur.stompRadius) return;

        EntityHp playerHp = minotaur.player.GetComponent<EntityHp>();
        if (playerHp != null)
        {
            playerHp.SetCustomKnockback(minotaur.stompKnockback, minotaur.stompKnockbackTime);
        }
        DamageScaleFactor scale = new DamageScaleFactor { physical = minotaur.stompDamageScale, elemental = 1f };
        minotaur.combat.PerformAttackOnTarget(minotaur.player, scale);
        hasDamaged = true;
    }
}