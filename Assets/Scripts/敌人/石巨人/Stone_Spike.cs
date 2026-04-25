using System.Collections;
using UnityEngine;

public class Stone_Spike : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsTarget;
    private Stone stone;
    private float delay;
    private Entity_Combat combat;
    private float spikeTime;
    private bool timeStart;


    private void Update()
    {
        if (timeStart)
        {
            spikeTime -= Time.deltaTime;
            if (spikeTime < 0)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // &位与，有0则0，<<左移符，LayerMask是32位int数，对应层级序号位为1
        //1的二进制左移layer序号位 然后异或 whatIsTarget 之后不为0说明存在同为1的位，即layer匹配
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            combat.PerformAttackOnTarget(collision.transform);
        }
    }
    public void SetSpike(Stone stone, int i)
    {
        this.stone = stone;
        spikeTime = stone.spikeTime;
        combat = stone.combat;
        delay = (i + 1) * stone.delayMultiplier;
        StartCoroutine(MoveSpike(i));
    }

    public IEnumerator MoveSpike(int i)
    {
        yield return new WaitForSeconds(delay);
        Vector2 targetPos = (Vector2)transform.position + new Vector2(0, 2);
        while (Vector2.Distance(targetPos, transform.position) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, stone.spikeMoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        timeStart = true;
        if (i == stone.spikesNumber - 1)
        {
            stone.attackOver = true;
        }

    }

}
