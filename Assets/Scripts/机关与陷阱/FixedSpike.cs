using UnityEngine;

public class FixedSpike : SpikeBase
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }
}