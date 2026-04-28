using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    private Image skillIcon;
    private RectTransform rect;
    private Button button;

    private Skill_DataSO skillData;

    public SkillType skilltype;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private string inputKeyName;
    [SerializeField] private TextMeshProUGUI inputKeyText;
    [SerializeField] public GameObject conflictSlot;
    public float timePassed;
    public Coroutine co;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        button = GetComponent<Button>();
        skillIcon = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }
    private void OnValidate()
    {
        gameObject.name = "UI-技能槽-" + skilltype.ToString();
    }
    public void SetupSkillSlot(Skill_DataSO selectedSkill)
    {
        skillData = selectedSkill;

        Color color = Color.black;
        color.a = .6f;
        cooldownImage.color = color;

        inputKeyText.text = inputKeyName;
        skillIcon.sprite = selectedSkill.icon;

        skillIcon.color = Color.white;  // <-- 添加这行，确保图标不透明

        if (conflictSlot != null)
            conflictSlot.SetActive(false);
        //此方法在UI树节点解锁默认技能时Unlock()被调用时才执行
        //而树节点的初始化方法Awake等需要节点对象激活及父对象技能树UI被激活才会调用
        //这会导致打开技能树才会更新游戏UI——技能槽
        //因此改用 UI 调用 技能树UI 实例的新默认技能解锁方法 直接调用各个子节点的默认技能解锁方法
        //通过其调用Unlock 最终调用本方法实现技能槽初始化
    }
    public void ResetCooldown()
    {
        cooldownImage.fillAmount = 0;
    }
    public void StartCooldown(float cooldown)
    {
        cooldownImage.fillAmount = 1;//图片的填充值设为1
        if (co != null)
            StopCoroutine(co);
        timePassed = 0;
        co = StartCoroutine(CooldownCo(cooldown));
    }
    public IEnumerator CooldownCo(float duration, float newTimePassed = 0)
    {
        timePassed = newTimePassed;
        // 立即更新填充，反映当前进度
        cooldownImage.fillAmount = 1f - (timePassed / duration);
        while (timePassed < duration)
        {
            timePassed = timePassed + Time.deltaTime;
            cooldownImage.fillAmount = 1f - (timePassed / duration);
            yield return null;
        }

        cooldownImage.fillAmount = 0;
        timePassed = duration;  // 确保与冷却完成一致
        co = null;              // 清除协程引用
    }
    public void newCo(float cooldown, float newTimePassed)
    {
        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(CooldownCo(cooldown, newTimePassed));
    }
    public void ClearSlot()
    {
        skillData = null;
        if (skillIcon != null)
        {
            skillIcon.sprite = null;
            skillIcon.color = new Color(1, 1, 1, 0); // 完全透明
        }
        inputKeyText.text = "";
        cooldownImage.fillAmount = 0;
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
        timePassed = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(skillData == null)
            return;
        ui.skillTooltip.ShowTooltip(true, rect, skillData, null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.ShowTooltip(false, null);

    }
}
