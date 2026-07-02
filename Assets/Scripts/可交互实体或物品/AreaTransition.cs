using UnityEngine;

public class AreaTransition : MonoBehaviour
{
    [Header("目标设置")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetExitId = "default";

    [Header("行为设置")]
    [SerializeField] private bool triggerOnlyOnce = true;
    [SerializeField] private LayerMask playerLayer;

    private bool hasTriggered = false;

    private void OnValidate()
    {
        if (playerLayer == 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerLayer = 1 << player.layer;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) == 0) return;
        if (triggerOnlyOnce && hasTriggered) return;
        if (string.IsNullOrEmpty(targetSceneName)) return;

        hasTriggered = true;
        GameSceneManager.instance.ChangeSceneWithAreaTransition(targetSceneName, targetExitId);
    }
}