using UnityEngine;

public class SkillObject_Hp : EntityHp
{
    protected override void Die()
    {
        SkillObject_Clone clone = GetComponent<SkillObject_Clone>();
        clone.HandleDeath();
    }
}
