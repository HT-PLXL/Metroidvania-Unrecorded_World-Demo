using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("弹反细节")]
    [cn("弹反后摇")][SerializeField] private float stunTimeAfterTryCounter = 0.25f;
    public bool tryCounter()//bool类型方法必须要有返回值，不然会标红
    {
        bool isCounter = false;
        foreach(var target in GetDetectColliders())
        {
            ICounter counterable = target.GetComponent<ICounter>();
            //定义接口实例并赋予从碰撞体中筛选出的附加了接口“弹反”的组件
            //如果没有获取到带有弹反接口的组件便为空
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
