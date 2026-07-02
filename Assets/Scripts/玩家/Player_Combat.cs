using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("弹反细节")]
    [cn("弹反检测时长")] public float Countertimer = 0.2f;
    [cn("弹反失败僵直")] public float stunTimeAfterTryCounter = 0.25f;
    [cn("能弹反什么")] public LayerMask whatIsCounterable;
    public bool tryCounter()//bool类型方法必须要有返回值
    {
        bool isCounter = false;
        foreach (var target in GetDetectColliders(whatIsCounterable))
        {
            ICounter counterable = target.GetComponent<ICounter>();

            if (counterable != null && counterable.CanBeCountered)
            {
                counterable.HandleCounter(); //运行接口实例的方法
                CounterDamage(target);
                isCounter = true;
            }
        }
        return isCounter;
    }
}
