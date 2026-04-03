using UnityEngine;
//弹反接口
public interface ICounter
{
    public bool CanBeCountered { get; }
    public void HandleCounter();
}
