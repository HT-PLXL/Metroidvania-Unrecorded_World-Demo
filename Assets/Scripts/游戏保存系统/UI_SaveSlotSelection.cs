using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SaveSlotSelection : MonoBehaviour
{
    [Header("槽位按钮数组 (按顺序0,1,2...)")]
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private TMP_Text[] slotInfoTexts;
    [Header("删除按钮数组 (每个槽位对应的删除按钮)")]
    [SerializeField] private Button[] deleteButtons;
    [Header("无存档时的提示文字")]
    [SerializeField] private string emptySlotText = "空存档";
    [Header("确认删除弹窗")]
    [SerializeField] private UI_ConfirmPopup confirmPopup; // 需在场景中预制体并拖拽

    private void OnEnable()
    {
        RefreshSlotDisplay();
    }

    private void RefreshSlotDisplay()
    {
        for (int i = 0; i < slotButtons.Length && i < SaveManager.instance.SlotCount; i++)
        {
            int slotIndex = i;
            bool hasSave = SaveManager.instance.GetSlotSaveExists(slotIndex);
            string displayText;

            if (hasSave)
            {
                var (gold, lastScene) = SaveManager.instance.GetSlotPreview(slotIndex);
                displayText = $"金币:{gold}\n场景:{(string.IsNullOrEmpty(lastScene) ? "未知" : lastScene)}";
            }
            else
            {
                displayText = emptySlotText;
            }

            // 更新文本
            if (slotInfoTexts != null && i < slotInfoTexts.Length && slotInfoTexts[i] != null)
                slotInfoTexts[i].text = displayText;

            // 绑定主按钮（点击加载/新建）
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(slotIndex, !hasSave));

            // 绑定删除按钮
            if (deleteButtons != null && i < deleteButtons.Length && deleteButtons[i] != null)
            {
                Button btn = deleteButtons[i];
                btn.onClick.RemoveAllListeners();
                if (hasSave)
                {
                    btn.gameObject.SetActive(true);
                    btn.interactable = true;
                    btn.onClick.AddListener(() => ShowDeleteConfirm(slotIndex));
                }
                else
                {
                    btn.gameObject.SetActive(false);
                }
            }
        }
    }

    private void ShowDeleteConfirm(int slotIndex)
    {
        if (confirmPopup == null)
        {
            Debug.LogError("未设置确认弹窗组件！");
            return;
        }
        confirmPopup.Show(message: $"确定要删除槽位 {slotIndex + 1} 的存档吗？", confirmAction: () => { SaveManager.instance.DeleteSlot(slotIndex); RefreshSlotDisplay(); }); // 显示文本；删除存档数据；刷新UI显示为空槽
    }

    private void OnSlotClicked(int slotIndex, bool isNewGame)
    {
        bool success;
        if (isNewGame)
            success = SaveManager.instance.CreateNewSlot(slotIndex);
        else
            success = SaveManager.instance.SelectSlot(slotIndex);

        if (success)
        {
            GameSceneManager.instance.ContinuePlay();
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"无法启动槽位 {slotIndex} 的游戏");
        }
    }
}