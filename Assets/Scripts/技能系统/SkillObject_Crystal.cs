using UnityEngine;

public class SkillObject_Crystal : SkillObject_Base
{
    [SerializeField][cn("Λ®Ύ§±¬Υ¨Τ€ΦΖΜε")] private GameObject vfxPrefab;
    public void SetUpCrystal(float time)
    {
        Invoke(nameof(Explode), time);
    }
    private void Explode()//±¬Υ¨
    {
        DamageEnemyInRadius(transform, checkRadius);
        Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;
        Explode();
    }
}
