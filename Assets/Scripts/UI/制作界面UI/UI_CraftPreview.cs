using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreview : MonoBehaviour
{
    private Bag_Item itemWillCraft;
    private Bag_Storage storage;
    private UI_CraftPreviewSlot[] craftPreviewSlots;

    private Coroutine co;

    [Header("Item Preview Setup")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private TextMeshProUGUI tooltipInfo;
    [SerializeField][cn("透明图标")] private Image transparentIcon;//透明图标

    private void OnDisable()//关闭时清理显示的内容，使它们恢复默认
    {
        itemWillCraft = null;
        itemIcon.sprite = transparentIcon.sprite;
        itemName.text = "";
        itemInfo.text = "请选择想要制作的物品";

        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void SetupCraftPreview(Bag_Storage storage)//首次设置
    {
        this.storage = storage;
        craftPreviewSlots = GetComponentsInChildren<UI_CraftPreviewSlot>(true);//不使用true会索引越界报错，应当是对象未激活的原因

        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }
    public void UpdateCraftPreview(ItemDataSO itemData)//更新可制作物品的具体信息
    {
        itemWillCraft = new Bag_Item(itemData);
        itemIcon.sprite = itemData.icon;
        itemName.text = itemData.itemName;
        itemInfo.text = itemWillCraft.GetItemInfo();

        UpdateCraftPreviewSlots();//更新配方槽
    }

    private void UpdateCraftPreviewSlots()//更新配方槽
    {
        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < itemWillCraft.itemData.craftRecipe.Length; i++)
        {
            Bag_Item requiredItem = itemWillCraft.itemData.craftRecipe[i];
            int availableAmount = storage.GetAvailableAmount(requiredItem.itemData);
            int requiredAmount = requiredItem.stackCount;

            craftPreviewSlots[i].gameObject.SetActive(true);
            craftPreviewSlots[i].SetupRecipeSlot(requiredItem.itemData, availableAmount, requiredAmount);
        }
    }

    public void ConfirmCraft()//确认合成
    {
        if (itemWillCraft == null)
        {
            tooltipInfo.text = "请选择制作目标";
            if (co != null)
                StopCoroutine(co);
            StartColseTooltipInfoCo();//变淡
            return;
        }
        if (storage.HasEnoughMaterials(itemWillCraft) && storage.bag.CanAddItem(itemWillCraft))//检查是否有足够材料以及是否能够添加（有空位/可堆叠）
        {
            storage.ConsumeMaterials(itemWillCraft);
            storage.bag.AddItem(itemWillCraft);
        }
        else
        {
            tooltipInfo.text = "材料不足或背包已满";
            if (co != null)
                StopCoroutine(co);
            StartColseTooltipInfoCo();//变淡
        }
        UpdateCraftPreviewSlots();//更新配方槽
    }
    private void StartColseTooltipInfoCo()
    {
        co = StartCoroutine(CloseTooltipInfo());
    }
    private IEnumerator CloseTooltipInfo()
    {
        Color color = Color.white;

        for (int i = 0; i < 12; i++)//反复延迟透明3s
        {
            color.a = color.a * 0.9f;
            tooltipInfo.color = color;
            yield return new WaitForSeconds(0.25f);
        }
        tooltipInfo.text = "";
    }


}
