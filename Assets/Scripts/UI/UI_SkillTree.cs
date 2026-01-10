using System.Security.Cryptography;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoint;
    [SerializeField] private UI_TreeConnectHandle[] parentNodes;


    public bool haveEnoughSkillPoint(int cost)=>skillPoint >= cost;
    public void RemoveSkillPoint(int cost)=>skillPoint -= cost;

    private void Start()
    {
        UpdateAllNodes();
    }

    //----------用于更新节点所有位置，使用菜单更新而不是OnValidate自动更新----------
    //防止节点放置失误，子节点放父节点套娃崩溃

    [ContextMenu("更新所有节点")]
    public void UpdateAllNodes()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllNode();
        }
    }
}

