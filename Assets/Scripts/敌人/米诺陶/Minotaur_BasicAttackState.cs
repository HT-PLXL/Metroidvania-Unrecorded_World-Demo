using UnityEngine;

public class Minotaur_BasicAttackState : EnemyState
{
    private Minotaur minotaur;
    private bool hasDamaged;

    public Minotaur_BasicAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName)
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

    public void PerformBasicDamage()
    {
        if (hasDamaged) return;
        hasDamaged = true;

        if (minotaur.player == null) return;

        float dist = Vector2.Distance(minotaur.transform.position, minotaur.player.position);
        if (dist > minotaur.attackDistance) return;

        EntityHp playerHp = minotaur.player.GetComponent<EntityHp>();
        if (playerHp != null)
        {
            playerHp.SetCustomKnockback(minotaur.basicKnockback, minotaur.basicKnockbackTime);
        }
        DamageScaleFactor scale = new DamageScaleFactor { physical = minotaur.basicDamageScale, elemental = 1f };
        minotaur.combat.PerformAttackOnTarget(minotaur.player, scale);
    }
}