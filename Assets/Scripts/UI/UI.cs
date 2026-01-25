using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_ItemTooltip itemToolTip;
    public UI_SkillTooltip skillTooltip;
    public UI_SkillTree skillTree;
    private bool skillTreeEnable;
    private void Awake()
    {
        itemToolTip = GetComponentInChildren<UI_ItemTooltip>();
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>();
        skillTree = GetComponentInChildren<UI_SkillTree>(true);//true会查找所有对象，包括被禁用的
    }
    public void ToggleSkillTreeUI()
    {
        skillTreeEnable = !skillTreeEnable;
        skillTree.gameObject.SetActive(skillTreeEnable);
        skillTooltip.ShowTooltip(false, null);
    }
    public void CloseSkillTreeUI()
    {
        skillTree.gameObject.SetActive(false);
        skillTooltip.ShowTooltip(false, null);
    }
}
