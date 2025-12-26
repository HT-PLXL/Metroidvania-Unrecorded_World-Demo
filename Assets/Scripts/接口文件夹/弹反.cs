using UnityEngine;

public interface 弹反
{
    public bool CanBeCountered { get; }
    public void HandleCounter();
}
