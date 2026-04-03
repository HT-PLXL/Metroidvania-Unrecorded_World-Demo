public class BringerOfDeath_AttackState : EnemyState
{
    private Enemy_BringerOfDeath bringerOfDeath;

    public BringerOfDeath_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        bringerOfDeath = enemy as Enemy_BringerOfDeath;
    }
    public override void Enter()
    {
        base.Enter();
        SetAttackSpeed();
    }
    public override void Update()
    {
        base.Update();
        if (triggerOver)
        {
            if (bringerOfDeath.CanTeleport())
                stateMachine.ChangeState(bringerOfDeath.bringerOfDeath_TeleportState);
            else
                stateMachine.ChangeState(bringerOfDeath.bringerOfDeath_BattleState);
        }
    }
}
