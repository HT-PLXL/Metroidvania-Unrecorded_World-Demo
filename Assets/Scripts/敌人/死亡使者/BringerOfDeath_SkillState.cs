using UnityEngine;

public class BringerOfDeath_SkillState : EnemyState
{
    private Enemy_BringerOfDeath bringerOfDeath;
    public BringerOfDeath_SkillState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        bringerOfDeath = enemy as Enemy_BringerOfDeath;
    }
    public override void Enter()
    {
        base.Enter();
        bringerOfDeath.SetVelocity(0, 0);
        bringerOfDeath.SetUseSkillPerformed(false);
        bringerOfDeath.IntoCooldown();
        bringerOfDeath.battleState.canNotJumpBack = true;
        bringerOfDeath.enemyHp.canEnterBattleState = false;
    }
    public override void Update()
    {
        base.Update();
        if (bringerOfDeath.useSkillPerformed)
            anim.SetBool("useSkillPerformed", true);
        if(triggerOver)
            stateMachine.ChangeState(bringerOfDeath.bringerOfDeath_BattleState);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool("useSkillPerformed", false);
        bringerOfDeath.battleState.canNotJumpBack = false;
        bringerOfDeath.enemyHp.canEnterBattleState = true;

    }
}
