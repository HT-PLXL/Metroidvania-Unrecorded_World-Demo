using UnityEngine;

public class Minotaur_ChargeState : EnemyState
{
    private Minotaur minotaur;
    private float chargeTimer;
    private bool hasDamaged;
    private Vector2 chargeDirection;

    public Minotaur_ChargeState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
        minotaur = enemy as Minotaur;
    }

    public override void Enter()
    {
        base.Enter();
        chargeTimer = minotaur.chargeDuration;
        hasDamaged = false;

        // 朝向玩家
        if (minotaur.player != null)
        {
            int dir = minotaur.DirectionToPlayer();
            if (dir != 0) minotaur.HandleFlip(dir);
            chargeDirection = new Vector2(dir, 0);
        }

        minotaur.vfx.DoEchoEffect(minotaur.chargeDuration);//残影

        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(chargeDirection.x * minotaur.chargeSpeed, 0);
    }

    public override void Update()
    {
        base.Update();
        rb.linearVelocity = new Vector2(chargeDirection.x * minotaur.chargeSpeed, rb.linearVelocity.y);

        // 检测地面：如果没有地面（即将掉落悬崖），立即停止冲撞
        if (!enemy.isDetectGround)
        {
            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            stateMachine.ChangeState(minotaur.battleState);
            return;
        }

        chargeTimer -= Time.deltaTime;
        if (chargeTimer <= 0 || triggerOver)
        {
            rb.gravityScale = 1;
            stateMachine.ChangeState(minotaur.battleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = 1;
    }

    public void PerformChargeDamage()
    {
        if (hasDamaged) return;

        if (minotaur.player == null) return;

        //攻击距离判定
        float distToPlayer = Vector2.Distance(minotaur.transform.position, minotaur.player.position);
        if (distToPlayer > minotaur.chargeHitDistance)
            return;

        // 获取玩家的血量组件
        EntityHp playerHp = minotaur.player.GetComponent<EntityHp>();
        if (playerHp != null)
        {
            playerHp.SetCustomKnockback(minotaur.chargeKnockback, 0.4f);
        }

        DamageScaleFactor scale = new DamageScaleFactor { physical = minotaur.chargeDamageScale, elemental = 1f };
        minotaur.combat.PerformAttackOnTarget(minotaur.player, scale);
        hasDamaged = true;
    }
}