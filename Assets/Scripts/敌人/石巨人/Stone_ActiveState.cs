public class Stone_ActiveState : EnemyState
{
    public Stone stone;
    public Stone_ActiveState(Stone stone, StateMachine stateMachine, string animBoolName) : base(stone, stateMachine, animBoolName)
    {
        this.stone = stone;
    }
    public override void Enter()
    {
        //base.Enter();
    }
    public override void Exit()
    {
        stone.boosHpBar.SetHpbar(stone);
        stone.shouldFloat = true;
    }
    public override void Update()
    {
        base.Update();
        if (triggerOver)
        {
            stone.stage = 1;
            anim.SetInteger("stage", 1);
            stateMachine.ChangeState(stone.battleState);
        }
    }
}
