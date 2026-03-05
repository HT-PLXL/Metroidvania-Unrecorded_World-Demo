using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;
    public List<Bag_Item> itemList;
    public SerializableDictionary<string, int> bag;
    public SerializableDictionary<string, int> storageItem;//物品储存
    public SerializableDictionary<string, int> storageMaterials;//材料储存
    public SerializableDictionary<string, ItemType> AlreadyEquipedItems;//已经装备上的物品

    public int skillPoints;
    public SerializableDictionary<string, bool> skillTreeUI;
    public SerializableDictionary<SkillType, SkillUpgradeType> skillUpgrades;

    public Vector3 saveCheckPoint;
    public GameData()
    {
        bag = new SerializableDictionary<string, int>();
        storageItem = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();

        AlreadyEquipedItems = new SerializableDictionary<string, ItemType>();
        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();
    }
    
}
