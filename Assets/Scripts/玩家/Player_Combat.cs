using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("弹反细节")]
    [cn("弹反后摇")][SerializeField] private float stunTimeAfterTryCounter = 0.25f;
    [cn("能弹反什么")][SerializeField] private LayerMask whatIsCounterable;
    public bool tryCounter()//bool类型方法必须要有返回值
    {
        bool isCounter = false;
        foreach(var target in GetDetectColliders(whatIsCounterable))
        {
            ICounter counterable = target.GetComponent<ICounter>();

            if (counterable != null && counterable.CanBeCountered)
            {
                counterable.HandleCounter(); //运行接口实例的方法
                isCounter = true;
            }
        }
        return isCounter;
    }
    public float GetStunTimeAfterTryCounter()
    {
        return stunTimeAfterTryCounter;
    }
}
