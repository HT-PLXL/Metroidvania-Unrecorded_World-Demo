using UnityEngine;

/// <summary>挂载在目标场景中传送出口位置，用于区域传送定位和自动走出</summary>
public class AreaTransitionExit : MonoBehaviour
{
    [Tooltip("与触发器中的出口标识匹配")]
    public string exitId = "default";

    [Tooltip("出去方向与速度")]
    public Vector2 moveDirectionAndSpeed = new Vector2(3f, 0f);

    [Tooltip("走出持续时间（秒）")]
    public float moveDuration = 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        if (moveDirectionAndSpeed != Vector2.zero)
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveDirectionAndSpeed.normalized * 1f);
        }
    }
}