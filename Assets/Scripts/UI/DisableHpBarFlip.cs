using UnityEngine;

//该类用于禁用血条翻转
public class DisableHpBarFlip : MonoBehaviour
{
    private Entity entity;
    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
    private void OnEnable()
    {
        entity.OnFlip += handleFlip;//给事件订阅新委托
    }
    private void OnDisable()
    {
        entity.OnFlip -= handleFlip;
    }
private void handleFlip()
    {
        transform.rotation = Quaternion.identity;//旋转值保持默认
    }
}
