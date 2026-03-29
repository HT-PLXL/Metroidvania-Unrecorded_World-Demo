using UnityEngine;

public class Player_DomainState : PlayerState
{
    private Vector2 originalPositon;
    private float originalGravity;
    private float finalRiseDistance;
    private bool isHovering;
    private bool canCreateDomain;
    public Player_DomainState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        originalPositon = player.transform.position;
        originalGravity = rb.gravityScale;
        finalRiseDistance = GetEffectiveRiseDistance();

        player.SetVelocity(0, player.riseSpeed);
        player.hp.CanTakeDamage(false);//使无敌
    }
    public override void Update()
    {
        base.Update();
        if (Vector2.Distance(originalPositon, player.transform.position) > finalRiseDistance && isHovering == false)
            Hover();
        if (isHovering)
        {
            skillManager.domain.HandleSpellCast();
            if (stateTimer < 0)//技能时间结束
            {
                isHovering = false;
                rb.gravityScale = originalGravity;
                stateMachine.ChangeState(player.idleState);
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        canCreateDomain = false;
        player.hp.CanTakeDamage(true);

    }
    private void Hover()
    {
        isHovering = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        stateTimer = skillManager.domain.GetDomainDuration();
        if (canCreateDomain == false)
        {
            canCreateDomain = true;
            skillManager.domain.CreateDomain();
        }
    }
    private float GetEffectiveRiseDistance()//判断和天花板距离
    {
        RaycastHit2D hit2D = Physics2D.Raycast(player.transform.position, Vector2.up, player.riseMaxDistance, player.whatIsGround);
        return hit2D.collider != null ? hit2D.distance - 1 : player.riseMaxDistance;
    }
}
