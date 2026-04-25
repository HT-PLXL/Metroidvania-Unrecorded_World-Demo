using System;
using UnityEngine;

public class StoneAnimationTrigger : EnemyAnimationTrigger
{
    private Stone stone;
    protected override void Awake()
    {
        base.Awake();

        stone = GetComponentInParent<Stone>();
    }
    private void CanTakeDamage()
    {
        stone.stoneHp.CanTakeDamage(true);
        stone.isTurning = false;
    }
    private void UseSpike()
    {
        stone.UseSpike();
    }
    private void Skill_Spike_Start()
    {
        stone.attackOver = false;
        stone.anim.SetBool("attackOver", false);
    }
    private void Skill_Laser_Start()
    {
        stone.isUsingSkill = true;
        stone.anim.SetBool("skillOver", false);

    }
}
