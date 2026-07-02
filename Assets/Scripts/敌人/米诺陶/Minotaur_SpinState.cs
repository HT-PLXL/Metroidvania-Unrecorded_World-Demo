using UnityEngine;

public class Minotaur_SpinState : EnemyState
{
    private Minotaur minotaur;
    private float spinTimer;

    public Minotaur_SpinState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
        minotaur = enemy as Minotaur;
    }

    public override void Enter()
    {
        base.Enter();
        spinTimer = minotaur.spinDuration;
        // 朝向玩家
        if (minotaur.player != null)
        {
            int dir = minotaur.DirectionToPlayer();
            if (dir != 0) minotaur.HandleFlip(dir);
        }
    }

    public override void Update()
    {
        base.Update();

        // 在旋转期间向玩家移动
        if (minotaur.player != null && minotaur.spinMoveSpeed > 0f && enemy.isDetectGround)
        {
            float distToPlayer = Vector2.Distance(minotaur.transform.position, minotaur.player.position);
            if (distToPlayer > 0.35f)
            {
                int dir = minotaur.DirectionToPlayer();
                float xVelocity = dir * minotaur.spinMoveSpeed;
                enemy.SetVelocity(xVelocity, rb.linearVelocity.y);
            }
            else
                enemy.SetVelocity(0, rb.linearVelocity.y);
        }

        spinTimer -= Time.deltaTime;
        if (spinTimer <= 0 || triggerOver)
            stateMachine.ChangeState(minotaur.battleState);
    }

    public void PerformSpinDamage()
    {
        if (minotaur.player == null) return;

        float dist = Vector2.Distance(minotaur.transform.position, minotaur.player.position);
        if (dist > minotaur.spinRadius) return;

        EntityHp playerHp = minotaur.player.GetComponent<EntityHp>();
        if (playerHp != null)
        {
            playerHp.SetCustomKnockback(minotaur.spinKnockback, minotaur.spinKnockbackTime);
        }
        DamageScaleFactor scale = new DamageScaleFactor { physical = minotaur.spinDamageScale, elemental = 1f };
        minotaur.combat.PerformAttackOnTarget(minotaur.player, scale);
    }
}