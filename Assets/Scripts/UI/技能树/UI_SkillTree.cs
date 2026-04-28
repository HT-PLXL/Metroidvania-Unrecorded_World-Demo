using System.Linq;
using TMPro;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour, ISaveable
{
    public int skillPoint;
    [SerializeField] private TextMeshProUGUI skillPointText;
    [SerializeField] private UI_TreeConnectHandle[] parentNodes;
    private UI_TreeNode[] allTreeNodes;

    public Player_SkillManager skillManager { get; private set; }


    private void Start()
    {
        UpdateAllNodes();
        UpdateSkillPointText();
    }

    private void UpdateSkillPointText()
    {
        skillPointText.text = skillPoint.ToString();
    }

    public void UnlockedDefaultSkills()//被UI的start调用，直接顺带把skillManager也初始化了
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();
        allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);
        foreach (var node in allTreeNodes)
            node.UnlockedDefaultSkill();
    }
    public bool HaveEnoughSkillPoint(int cost) => skillPoint >= cost;
    public void RemoveSkillPoint(int cost)
    {
        skillPoint -= cost;
        UpdateSkillPointText();
    }
    public void AddSkillPoint(int cost)
    {
        skillPoint += cost;
        UpdateSkillPointText();
    }
    public int GetSkillPoints() => skillPoint;

    [ContextMenu("重置技能")]
    public void RefundAllSkillPoint()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();
        foreach (var node in skillNodes)
            node.RefundSkillPoint();

        // 重新应用默认技能，刷新 UI 图标
        UnlockedDefaultSkills();
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
        data.skillPoints = skillPoint;
        data.skillTreeUI.Clear();
        data.skillTypeAndUpgrades.Clear();

        allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.skillName;//保存键-技能名
            data.skillTreeUI[skillName] = node.isUnlocked;//保存值-是否解锁
        }
        foreach (var skill in skillManager.allSkillType)
        {
            data.skillTypeAndUpgrades[skill.GetSkillType()] = skill.GetUpgradeType();
        }
    }
    public void loadData(GameData data)
    {
        skillPoint = data.skillPoints;

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.skillName;

            if (data.skillTreeUI.TryGetValue(skillName, out bool unlocked) && unlocked)
                node.UnlockOnLoadData();
        }
        foreach (var skillType in skillManager.allSkillType)//遍历技能种类
        {
            if (data.skillTypeAndUpgrades.TryGetValue(skillType.GetSkillType(), out SkillUpgradeType upgradeType))//如果字典里存在对应的种类（顺带传出升级种类）
            {
                var upgradeNode = allTreeNodes.FirstOrDefault(node => node.skillData.upgradeData.upgradeType == upgradeType);//获取存在这种升级种类的子节点
                if (upgradeNode != null)
                    skillType.SetSkillUpgrade(upgradeNode.skillData);//使用子节点的Skill_DataSO数据进行设置
            }
        }
    }

}

