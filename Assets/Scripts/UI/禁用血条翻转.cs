using UnityEngine;

public class 禁用血条翻转 : MonoBehaviour
{
    private Entity entity;
    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
    private void OnEnable()
    {
        entity.翻转中 += 处理翻转;//给事件订阅新委托
    }
    private void OnDisable()
    {
        entity.翻转中 -= 处理翻转;
    }
private void 处理翻转()
    {
        transform.rotation = Quaternion.identity;//旋转值保持默认
    }
}
