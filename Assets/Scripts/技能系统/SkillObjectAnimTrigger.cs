using UnityEngine;

public class SkillObjectAnimTrigger : MonoBehaviour
{
    private SkillObject_Clone clone;
    private void Awake()
    {
        clone = GetComponentInParent<SkillObject_Clone>();
    }
    private void AttackTrigger()
    {
        clone.PerformAttack();
    }
    private void TryToEndSkill(int currentAttackIndex)
    {
        if(currentAttackIndex== clone.maxAttackCounts)
            clone.HandleDeath();
    }
}
