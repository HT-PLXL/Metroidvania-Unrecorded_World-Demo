using System.Collections.Generic;
using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance { get; private set; }

    [SerializeField] private DamagePopup popupPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<DamagePopup> pool = new Queue<DamagePopup>();
    private Dictionary<Transform, List<DamagePopup>> activePopupsPerTarget = new Dictionary<Transform, List<DamagePopup>>();
    [SerializeField] private int maxPopupsPerTarget = 5; // 同一个目标最大同时显示数
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            DamagePopup popup = Instantiate(popupPrefab, transform);
            popup.gameObject.SetActive(false);
            pool.Enqueue(popup);
        }
    }

    public void ShowDamage(Vector3 worldPosition, float damage, ElementType element, bool isCrit, Transform followTarget = null)
    {
        DamagePopup popup = null;

        // 如果跟随目标已存在过多跳字，尝试累加到最近的一个
        if (followTarget != null && activePopupsPerTarget.TryGetValue(followTarget, out var list) && list.Count >= maxPopupsPerTarget)
        {
            // 找到最新的（最后添加的）跳字
            var lastPopup = list[list.Count - 1];
            lastPopup.AddDamage(damage, isCrit); // 需要给 DamagePopup 增加 AddDamage 方法
            return;
        }

        popup = GetFromPool();
        popup.Show(damage, element, isCrit, worldPosition, followTarget);

        // 记录活跃关系
        if (followTarget != null)
        {
            if (!activePopupsPerTarget.ContainsKey(followTarget))
                activePopupsPerTarget[followTarget] = new List<DamagePopup>();
            activePopupsPerTarget[followTarget].Add(popup);
        }
    }

    public void ShowHeal(Vector3 worldPosition, float healAmount, Transform followTarget = null)
    {
        DamagePopup popup = GetFromPool();
        popup.ShowHeal(healAmount, worldPosition, followTarget);
    }

    private DamagePopup GetFromPool()
    {
        if (pool.Count > 0)
        {
            DamagePopup popup = pool.Dequeue();
            popup.gameObject.SetActive(true);
            return popup;
        }

        DamagePopup newPopup = Instantiate(popupPrefab, transform);
        return newPopup;
    }

    public void ReturnToPool(DamagePopup popup)
    {
        // 从活跃字典中移除
        if (popup.FollowTarget != null && activePopupsPerTarget.TryGetValue(popup.FollowTarget, out var list))
            list.Remove(popup);
        popup.ResetState();
        pool.Enqueue(popup);
    }
}