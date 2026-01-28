using System.Linq;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour, ISaveable
{
    [SerializeField] private int skillPoint;
    [SerializeField] private UI_TreeConnectHandle[] parentNodes;
    private UI_TreeNode[] allTreeNodes;

    public Player_SkillManager skillManager { get; private set; }
    private void Awake()
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();


    }
    private void Start()
    {
        UpdateAllNodes();

    }
    public bool HaveEnoughSkillPoint(int cost) => skillPoint >= cost;
    public void RemoveSkillPoint(int cost) => skillPoint -= cost;
    public void AddSkillPoint(int cost) => skillPoint += cost;

    [ContextMenu("重置技能")]
    public void RefundAllSkillPoint()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();
        foreach (var node in skillNodes)
            node.RefundSkillPoint();
    }


    //----------用于更新节点所有位置，使用菜单更新而不是OnValidate自动更新----------
    //防止节点放置失误，子节点放父节点套娃崩溃

    [ContextMenu("更新所有节点")]
    public void UpdateAllNodes()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllNode();
        }
    }

    public void SavaData(ref GameData data)
    {
        //data.skillPoints = skillPoint;
        //data.skillTreeUI.Clear();
        //data.skillUpgrades.Clear();

        //allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);
        //Debug.Log($"{allTreeNodes.Length}");

        //foreach (var node in allTreeNodes)
        //{
        //    string skillName = node.skillData.skillName;
        //    data.skillTreeUI[skillName] = node.isUnlocked;
        //}
        //foreach (var skill in skillManager.allSkills)
        //{
        //    data.skillUpgrades[skill.GetSkillType()] = skill.GetUpgradeType();
        //}
    }
    public void loadData(GameData data)
    {
        //skillPoint = data.skillPoints;

        //foreach (var node in allTreeNodes)
        //{
        //    string skillName = node.skillData.skillName;

        //    if (data.skillTreeUI.TryGetValue(skillName, out bool unlocked) && unlocked)
        //        node.UnlockByUsingSaveData();
        //}
        //foreach (var skill in skillManager.allSkills)
        //{
        //    if (data.skillUpgrades.TryGetValue(skill.GetSkillType(), out SkillUpgradeType upgradeType))
        //    {
        //        var upgradeNode = allTreeNodes.FirstOrDefault(node => node.skillData.upgradeData.upgradeType == upgradeType);
        //        if (upgradeNode != null)
        //            skill.SetSkillUpgrade(upgradeNode.skillData.upgradeData);
        //        //skill.SetSkillUpgrade(upgradeNode.skillData);记得改
        //    }
        //}
    }

}

