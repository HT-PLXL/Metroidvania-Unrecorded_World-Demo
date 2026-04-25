public class Stone_AttackState : EnemyState
{
    public Stone stone;
    public Stone_AttackState(Stone stone, StateMachine stateMachine, string animBoolName) : base(stone, stateMachine, animBoolName)
    {
        this.stone = stone;
    }
    public override void Enter()
    {
        base.Enter();
        stone.SetVelocity(0, rb.linearVelocity.y);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetInteger("attackIndex", 0);
    }
    public override void Update()
    {
        base.Update();
        if (triggerOver)
        {
            stateMachine.ChangeState(stone.battleState);
        }
        if (stone.isUsingSkill)
        {
            stone.isUsingSkill = false;
            stone.CreateLaser();
        }
        if (stone.skillOver)
        {
            stone.skillOver = false;
            anim.SetBool("skillOver", true);
            stateMachine.ChangeState(stone.battleState);
        }
    }
}
