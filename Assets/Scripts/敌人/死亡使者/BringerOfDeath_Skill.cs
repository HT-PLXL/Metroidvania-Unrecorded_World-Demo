using UnityEngine;

public class BringerOfDeath_Skill : MonoBehaviour
{
    private Entity_Combat combat;
    private DamageScaleFactor damageScaleFactor;
    [SerializeField] private LayerMask whoIsTarget;
    [SerializeField] private Collider2D col;
    private void EnableCollider() => col.enabled = true;
    private void DisableCollider() => col.enabled = false;
    public void SetSkillObject(Entity_Combat combat, DamageScaleFactor damageScaleFactor)
    {
        this.damageScaleFactor = damageScaleFactor;
        this.combat = combat;
        Destroy(gameObject, 1.5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // &位与，有0则0，<<左移符，LayerMask是32位int数，对应层级序号位为1
        //1的二进制左移layer序号位 然后异或 whatIsTarget 之后不为0说明存在同为1的位，即layer匹配
        if (((1 << collision.gameObject.layer) & whoIsTarget) != 0)
        {
            //检查combat是否已被销毁
            if (combat != null)
            {
                combat.PerformAttackOnTarget(collision.transform, damageScaleFactor);
                DisableCollider();
            }
            else
            {
                //攻击者销毁后，销毁技能
                Destroy(gameObject);
            }
        }
    }
}
