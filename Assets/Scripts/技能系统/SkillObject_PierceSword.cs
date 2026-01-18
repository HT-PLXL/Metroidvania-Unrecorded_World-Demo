using UnityEngine;

public class SkillObject_PierceSword : SkillObject_Sword
{
    private int pierceNumber;
    public override void SetUpSword(Skill_ThrowSword throwSword, Vector2 direction)
    {
        base.SetUpSword(throwSword, direction);
        pierceNumber = throwSword.canPierceNumber;
    }
    protected override void OnTriggerEnter2D(Collider2D collider)//飞行时穿透与伤害判定
    {
        bool HitGround = collider.gameObject.layer == LayerMask.NameToLayer("地面");
        if(pierceNumber <= 0|| HitGround)
        {
            DamageEnemyInRadius(transform, checkRadius);
            StopSword(collider);
            return;
        }
        pierceNumber--;
        DamageEnemyInRadius(transform, checkRadius);
    }
}
