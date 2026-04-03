public class BringerOfDeath_TeleportState : EnemyState
{
    private Enemy_BringerOfDeath bringerOfDeath;
    public BringerOfDeath_TeleportState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        bringerOfDeath = enemy as Enemy_BringerOfDeath;
    }
    public override void Enter()
    {
        base.Enter();
        bringerOfDeath.CanBeSelect(false);
    }
    public override void Update()
    {
        base.Update();
        if (bringerOfDeath.teleportTrigger)
        {
            bringerOfDeath.transform.position = bringerOfDeath.FindTeleportPoint();
            bringerOfDeath.SetTeleportTrigger(false);
        }
        if (triggerOver)
        {
            if (bringerOfDeath.CanUseSkill())
                stateMachine.ChangeState(bringerOfDeath.bringerOfDeath_SkillState);
            else
                stateMachine.ChangeState(bringerOfDeath.bringerOfDeath_BattleState);
        }
    }
    public override void Exit()
    {
        base.Exit();
        bringerOfDeath.CanBeSelect(true);
    }
}
