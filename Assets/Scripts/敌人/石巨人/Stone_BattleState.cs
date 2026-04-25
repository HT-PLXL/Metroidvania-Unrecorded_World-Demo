using UnityEngine;

public class Stone_BattleState : Enemy_BattleState
{
    public Stone stone;
    public Stone_BattleState(Stone stone, StateMachine stateMachine, string animBoolName) : base(stone, stateMachine, animBoolName)
    {
        this.stone = stone;
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimator();


        if (BattleTimeOver())//战斗时间结束剧情杀
            KillPlayer();


        if (stone.stage == 1 && InAttackRange() && CanAttack())
        {
            lastTimeAttack = Time.time;
            anim.SetInteger("attackIndex", 1);//如果1阶段攻击时，stage变为2，
            stateMachine.ChangeState(stone.attack);

        }
        else if (stone.stage == 2 && InAttackRange() && stone.DetectPlayer() && stone.isTurning == false)
        {

            if (CanUseSkill())
            {
                anim.SetInteger("attackIndex", 2);
                stateMachine.ChangeState(stone.attack);
            }
            else
            {
                anim.SetInteger("attackIndex", 1);//普通攻击
                stateMachine.ChangeState(stone.attack);
            }
        }
        else if (stone.stage == 3 && InAttackRange() && stone.DetectPlayer() && stone.isTurning == false)
        {
            anim.SetInteger("attackIndex", 1);//普通攻击
            stateMachine.ChangeState(stone.attack);
        }
        else
        {
            if (Mathf.Abs(stone.transform.position.x - player.transform.position.x) > 0.35f)
            {
                float xVelocity = enemy.ShouldChasePlayer ? enemy.GetBattleMoveSpeed() : 0.0001f;//使用一个极小数，确保几乎不动的同时也能正常翻转
                stone.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocity.y);
            }
            else
            {
                stone.SetVelocity(0.0001f * DirectionToPlayer(), rb.linearVelocity.y);
            }
        }

    }
    private bool CanUseSkill()
    {
        if (Time.time > stone.lastTimeUseSkill + stone.skillCooldown)
            return true;
        return false;
    }

    public void KillPlayer()
    {

    }

}
