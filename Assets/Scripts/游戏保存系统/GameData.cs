using System;
using System.Collections.Generic;
using UnityEngine;
//这个类包含所有需要保存的信息
[Serializable]
public class GameData
{
    public int gold;
    public List<Bag_Item> itemList;
    public SerializableDictionary<string, int> bag;//<物品ID，堆叠数量>
    public SerializableDictionary<string, int> storageItem;//物品仓库
    public SerializableDictionary<string, int> storageMaterials;//材料仓库
    public SerializableDictionary<string, ItemType> AlreadyEquipedItems;//已经装备上的物品 <id，槽位种类>

    public int skillPoints;
    public SerializableDictionary<string, bool> skillTreeUI;//<名字，是否解锁>
    public SerializableDictionary<SkillType, SkillUpgradeType> skillTypeAndUpgrades;//<技能种类，升级进度>

    public SerializableDictionary<string, bool> unlockedCheckPoints;//保存的重生点的位置
    public SerializableDictionary<string, Vector3> scenePortals;//<场景名，传送门坐标>
    public string portalToSceneName;
    public bool returnFormTown;

    public string lastScenePlayed;
    public Vector3 lastPlayerPosition;
    public GameData()
    {
        bag = new SerializableDictionary<string, int>();
        storageItem = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();

        AlreadyEquipedItems = new SerializableDictionary<string, ItemType>();
        skillTreeUI = new SerializableDictionary<string, bool>();
        skillTypeAndUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();
        unlockedCheckPoints = new SerializableDictionary<string, bool>();
        scenePortals = new SerializableDictionary<string, Vector3>();
    }
    
}
