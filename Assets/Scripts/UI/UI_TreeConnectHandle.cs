using System;
using UnityEngine;

[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandle childNode;
    public NodeDirectionType direction;
    [Range(100f, 300f)] public float length;
}

public class UI_TreeConnectHandle : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] details;

    [SerializeField] private UI_TreeConnection[] connections;

    private void OnValidate()
    {
        if (details.Length <= 0)
            return;
        if (details.Length != connections.Length)
        {
            Debug.Log("细节数量和连接数量应该相等" + gameObject.name);
            return;
        }
        UpdateConnection();
    }
    private void UpdateConnection()
    {
        for (int i = 0; i < details.Length; i++)
        {
            var detail = details[i];
            var connection = connections[i];

            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            connection.DirectConnection(detail.direction, detail.length);
            detail.childNode?.SetPosition(targetPosition);
        }
    }
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
