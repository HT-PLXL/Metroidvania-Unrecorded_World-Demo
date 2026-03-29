using UnityEngine;
using UnityEngine.UI;

public class UI_Storage : MonoBehaviour
{
    private Bag_Player bag;
    private Bag_Storage storage;
    [SerializeField]private UI_ItemSlotParent bagParent;
    [SerializeField]private UI_ItemSlotParent StorageParent;
    [SerializeField]private UI_ItemSlotParent materialStorageParent;

    public void SetupStorageUI(Bag_Storage storage)
    {
        this.storage = storage;
        bag = storage.bag;
        storage.OnBagChange += UpdateUI;
        UpdateUI();

        UI_StorageSlot[] storageSlots = GetComponentsInChildren<UI_StorageSlot>();

        foreach (var slot in storageSlots)
        {
            slot.SetStorage(storage);
        }
    }
    private void OnEnable()
    {
        UpdateUI();
    }
    private void UpdateUI()
    {
        if(storage == null)
            return;
        bagParent.UpdateSlots(bag.itemList);
        StorageParent.UpdateSlots(storage.itemList);
        materialStorageParent.UpdateSlots(storage.materialList);
    }
}
