using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_ItemTooltip itemToolTip { get; private set; }
    public UI_SkillTooltip skillTooltip { get; private set; }
    public UI_StatTooltip statsTooltip { get; private set; }
    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Bag bagUI { get; private set; }
    private bool skillTreeEnable;
    private bool bagEnable;
    private void Awake()
    {
        itemToolTip = GetComponentInChildren<UI_ItemTooltip>();
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>();
        statsTooltip = GetComponentInChildren<UI_StatTooltip>();
        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);//true会查找所有对象，包括被禁用的
        bagUI = GetComponentInChildren<UI_Bag>(true);

        skillTreeEnable = skillTreeUI.gameObject.activeSelf;//同步激活情况，防止出现初始激活，然后需要两次才能关掉
        bagEnable = bagUI.gameObject.activeSelf;
    }
    public void ToggleSkillTreeUI()
    {
        skillTreeEnable = !skillTreeEnable;

        if (skillTreeEnable && bagEnable)//先关闭其他UI
            ToggleBagUI();

        skillTreeUI.gameObject.SetActive(skillTreeEnable);
        skillTooltip.ShowTooltip(false, null);
    }
    public void ToggleBagUI()
    {
        bagEnable = !bagEnable;

        if (skillTreeEnable && bagEnable)//先关闭其他UI
            ToggleSkillTreeUI();

        bagUI.gameObject.SetActive(bagEnable);
        statsTooltip.ShowTooltip(false, null);
        itemToolTip.ShowTooltip(false, null);
    }
    public void CloseUI()
    {
        skillTreeEnable = true;//先设为真，方法内会反转，从而关闭
        ToggleSkillTreeUI();

        bagEnable = true;//先设为真，方法内会反转，从而关闭
        ToggleBagUI();
    }
}
