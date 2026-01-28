using UnityEngine;

//这个脚本用于管理玩家的技能，写新技能种类需要来这里更新
//创建新技能种类的实例，获取组件，更新下方方法-根据技能种类返回实例用于调用
public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_ThrowSword throwSword { get; private set; }
    public Skill_Clone clone { get; private set; }
    public Skill_Domain domain {  get; private set; }
    public Skill_Base[] allSkills {  get; private set; }

    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        throwSword = GetComponentInChildren<Skill_ThrowSword>();
        clone = GetComponentInChildren<Skill_Clone>();
        domain = GetComponentInChildren<Skill_Domain>();
        allSkills = GetComponentsInChildren<Skill_Base>();
    }
    public void ReduceAllSkillCooldown(float amount)
    {
        foreach(var skill in allSkills)
            skill.ReduceCooldown(amount);
    }
    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.冲刺: return dash;//返回实例（技能）
            case SkillType.魔能晶体: return shard;
            case SkillType.掷剑: return throwSword;
            case SkillType.克隆: return clone;
            case SkillType.领域:return domain;


            default:
                Debug.Log($"尚未实现的技能{type}");
                return null;
        }
    }
}
