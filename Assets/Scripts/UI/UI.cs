using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_SkillTooltip skillTooltip;
    public UI_SkillTree skillTree;
    private bool skillTreeEnable;
    private void Awake()
    {
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>();
        skillTree = GetComponentInChildren<UI_SkillTree>(true);//true会查找所有对象，包括被禁用的
    }
    public void ToggleSkillTreeUI()
    {
        skillTreeEnable = !skillTreeEnable;
        skillTree.gameObject.SetActive(skillTreeEnable);
        skillTooltip.showTooltip(false, null);
    }
    public void CloseSkillTreeUI()
    {
        skillTree.gameObject.SetActive(false);
        skillTooltip.showTooltip(false, null);
    }
}
