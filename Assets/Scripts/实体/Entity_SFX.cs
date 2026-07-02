using UnityEngine;

public class Entity_SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("音效名")]
    [SerializeField][cn("攻击命中")] private string attackHit;
    [SerializeField][cn("攻击未命中")] private string attackMiss;
    [SerializeField][cn("远程攻击")] private string RemoteAttack;
    [SerializeField][cn("声音传播范围")] private float soundDistance = 15f;
    [SerializeField][cn("显示声音传播范围圆圈")] private bool showGizmo;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void PlayAttackHit()
    {
        AudioManager.instance.PlaySFX(attackHit, audioSource, soundDistance);
    }

    public void PlayAttackMiss()
    {
        AudioManager.instance.PlaySFX(attackMiss, audioSource, soundDistance);
    }

    public void PlaySpecialAttack()//特殊攻击使用
    {
        AudioManager.instance.PlaySFX(RemoteAttack, audioSource, soundDistance);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, soundDistance);
        }
    }
}
