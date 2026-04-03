using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandle childNode;
    public NodeDirectionType direction;
    [Range(100f, 300f)] public float length;//连接线长度
    [Range(-45f, 45f)] public float rotation;//连接线额外左右偏移量
}

public class UI_TreeConnectHandle : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    public UI_TreeConnectDetails[] connectDetails;

    [SerializeField] private UI_TreeConnection[] connections;

    private Image connectionLineImage;
    private Color OriginalColor;//原始颜色
    private static bool allowSetAsLastSibling = false;//设置为static静态，使所有实例共享一个bool值
    private void Awake()
    {
        if (connectionLineImage != null)
            OriginalColor = connectionLineImage.color;
    }

    private void OnValidate()//编辑器专属方法，此处用于修改参数时自动执行移动节点相关功能
    {
        if (connectDetails.Length <= 0)
            return;
        if (connectDetails.Length != connections.Length)
        {
            Debug.Log("细节数量和连接数量应该相等" + gameObject.name);
            return;
        }
        UpdateConnection();
    }
    public UI_TreeNode[] GetChildNodes()//获取子节点
    {
        List<UI_TreeNode> returnChildNode = new List<UI_TreeNode>();
        foreach(var node in connectDetails)
        {
            if(node.childNode != null)
                returnChildNode.Add(node.childNode.GetComponent<UI_TreeNode>());
        }
        return returnChildNode.ToArray();
    }
    public void UpdateConnection()
    {
        for (int i = 0; i < connectDetails.Length; i++)
        {
            var detail = connectDetails[i];
            var connection = connections[i];

            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            Image connectionLine = connection.GetConnectionLineImage();

            connection.DirectConnection(detail.direction, detail.length, detail.rotation);//控制对象“连接”的方向长度旋转

            if (detail.childNode == null)//此处用于避免控制台报错，使childNode可以为空
                continue;
            detail.childNode.SetPosition(targetPosition);
            detail.childNode.SetConnectionLineImage(connectionLine);
            if (allowSetAsLastSibling)
                detail.childNode.transform.SetAsLastSibling();//将子节点设为最后一个对象，配合遍历可修复层级错误
        }
    }
    public void UpdateAllNode()
    {
        allowSetAsLastSibling = true;
        UpdateConnection();//更新主节点连接线相关及子节点位置
        foreach (var node in connectDetails)//每个技能都是主节点，但也可以被其他节点认为是“子节点”
        {
            if (node.childNode == null)//防报错
                continue;
            node.childNode?.UpdateConnection();//更新子节点及
        }
        allowSetAsLastSibling = false;
    }
    public void UnlockConnectionLineImage(bool unlock)
    {
        if (connectionLineImage == null)
            return;
        connectionLineImage.color = unlock ? Color.white : OriginalColor;
    }
    public void SetConnectionLineImage(Image image) => connectionLineImage = image;
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;//改变锚点位置
}
