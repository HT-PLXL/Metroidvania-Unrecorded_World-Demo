using UnityEngine;
//制作面版-种类木牌的脚本
//unity选择对应类别可制作物品列表
//种类有多个，但槽位只有一组,点击对应木牌触发button，执行更新方法
//该方法会关闭所有槽位，重新展示对应类别的可制作物品
public class UI_CraftListButton : MonoBehaviour
{
    [SerializeField] private ItemListDataSO craftData;//unity选择-菜单生成的某类可制作物品列表
    private UI_CraftSlot[] craftSlots;//所有槽位数组

    public void SetCraftSlots(UI_CraftSlot[] craftSlots) => this.craftSlots = craftSlots;

    public void UpdateCraftSlots()//buttom组件绑定此方法，点击种类木牌时触发
    {
        if (craftData == null)
        {
            Debug.Log("你需要分配可制作物品列表");
            return;
        }

        foreach (var slot in craftSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < craftData.itemList.Length; i++)//展示可制作目标
        {
            ItemDataSO itemData = craftData.itemList[i];
            craftSlots[i].gameObject.SetActive(true);
            craftSlots[i].SetupButton(itemData);
        }
    }
}
